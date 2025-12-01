using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.ECTS.Services;
using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.DoctoralProjects.Commands
{
    public class ReviewDoctoralProjectHandler : IRequestHandler<ReviewDoctoralProjectCommand, ReviewDoctoralProjectResponse>
    {
        private readonly IDoctoralProjectRepository _projectRepository;
        private readonly IEctsTrackingRepository _ectsTrackingRepository;
        private readonly EctsProgressService _ectsProgressService;
        private readonly ICurrentUserService _currentUserService;

        public ReviewDoctoralProjectHandler(IDoctoralProjectRepository projectRepository, IEctsTrackingRepository ectsTrackingRepository, EctsProgressService ectsProgressService, ICurrentUserService currentUserService)
        {
            _projectRepository = projectRepository;
            _ectsTrackingRepository = ectsTrackingRepository;
            _ectsProgressService = ectsProgressService;
            _currentUserService = currentUserService;
        }

        public async Task<ReviewDoctoralProjectResponse> Handle(ReviewDoctoralProjectCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            var currentUserRole = _currentUserService.Role;

            if (currentUserRole != "Admin" && currentUserRole != "Committee" && currentUserRole != "Mentor")
            {
                throw new Exception("Only users with Admin, Committee, or Mentor roles can review doctoral projects.");
            }

            var project = await _projectRepository.GetByIdAsync(request.ProjectId)
                ?? throw new Exception($"Doctoral project with id {request.ProjectId} not found.");

            if (project.Status != ProjectStatus.Submitted && project.Status != ProjectStatus.UnderReview && project.Status != ProjectStatus.ChangesRequested)
            {
                throw new Exception("Only projects in status: Submitted, UnderReview, ChangesRequested can be reviewed.");
            }

            if (!IsValidTransition(project.Status, request.NewStatus))
            {
                throw new Exception($"Invalid status transition from {project.Status} to {request.NewStatus}.");
            }

            var proposalDoc = project.Documents?.FirstOrDefault(d => d.DocumentType == ActivityDocumentType.DoctoralProjectReport);
            if (proposalDoc == null)
            {
                throw new Exception("Doctoral project proposal document is missing.");
            }

            if (request.NewStatus == ProjectStatus.Approved && proposalDoc.Status != DocumentStatus.Approved)
            {
                throw new Exception("Cannot approve project — proposal document must be approved first.");
            }

            if (request.DocumentStatus.HasValue)
            {
                proposalDoc.Status = request.DocumentStatus.Value;
                proposalDoc.ReviewComment = request.ReviewComment;
                proposalDoc.ReviewedAt = DateTime.UtcNow;
                proposalDoc.ReviewedBy = currentUserId;
                await _projectRepository.UpdateAsync(project);
            }

            project.Status = request.NewStatus;
            project.CommitteeNotes = request.CommitteeNotes;

            if (request.NewStatus == ProjectStatus.Approved ||
                request.NewStatus == ProjectStatus.Rejected)
            {
                project.DecisionAt = DateTime.UtcNow;
            }

            if (request.NewStatus == ProjectStatus.Approved)
            {
                var ectsTracking = await _ectsTrackingRepository.GetByStudentIdAsync(project.StudentId);
                if (ectsTracking != null)
                {
                    ectsTracking.IndependentResearchProject = 14;
                    await _ectsTrackingRepository.UpdateAsync(ectsTracking);

                    await _ectsProgressService.UpdateStudentSemesterAsync(project.StudentId, ectsTracking.TotalECTS);
                }
                else
                {
                    throw new Exception("ECTS tracking record not found for this student");
                }
            }

            await _projectRepository.UpdateAsync(project);

            return new ReviewDoctoralProjectResponse
            {
                Id = project.Id,
                Title = project.Title,
                Status = project.Status.ToString(),
                CommitteeNotes = project.CommitteeNotes,
                DecisionAt = project.DecisionAt
            };
        }

        private bool IsValidTransition(ProjectStatus current, ProjectStatus next)
        {
            var valid = new Dictionary<ProjectStatus, List<ProjectStatus>>
            {
                [ProjectStatus.Submitted] = new() { ProjectStatus.UnderReview, ProjectStatus.Rejected },
                [ProjectStatus.UnderReview] = new() { ProjectStatus.Approved, ProjectStatus.ChangesRequested, ProjectStatus.Rejected },
                [ProjectStatus.ChangesRequested] = new() { ProjectStatus.UnderReview, ProjectStatus.Approved, ProjectStatus.Rejected }
            };

            return valid.ContainsKey(current) && valid[current].Contains(next);
        }
    }
}
