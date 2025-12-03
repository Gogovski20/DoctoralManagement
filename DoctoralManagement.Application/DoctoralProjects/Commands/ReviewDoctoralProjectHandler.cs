using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.ECTS.Services;
using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DoctoralManagement.Application.DoctoralProjects.Commands
{
    public class ReviewDoctoralProjectHandler : IRequestHandler<ReviewDoctoralProjectCommand, ReviewDoctoralProjectResponse>
    {
        private readonly IDoctoralProjectRepository _projectRepository;
        private readonly IEctsTrackingRepository _ectsTrackingRepository;
        private readonly EctsProgressService _ectsProgressService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;
        private readonly ILogger<ReviewDoctoralProjectHandler> _logger;

        public ReviewDoctoralProjectHandler(IDoctoralProjectRepository projectRepository, IEctsTrackingRepository ectsTrackingRepository, EctsProgressService ectsProgressService, ICurrentUserService currentUserService, IAuthService authService, ILogger<ReviewDoctoralProjectHandler> logger)
        {
            _projectRepository = projectRepository;
            _ectsTrackingRepository = ectsTrackingRepository;
            _ectsProgressService = ectsProgressService;
            _currentUserService = currentUserService;
            _authService = authService;
            _logger = logger;
        }

        public async Task<ReviewDoctoralProjectResponse> Handle(ReviewDoctoralProjectCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            var currentUserRole = _currentUserService.Role;
            var allowedRoles = new[] { "Admin", "Committee", "Mentor", "Secretary" };

            if (!allowedRoles.Contains(currentUserRole))
            {
                throw new DoctoralManagementException(
                    "Only admins, committee members, mentors, or secretaries can review doctoral projects.",
                    HttpStatusCode.Forbidden);
            }

            var project = await _projectRepository.GetByIdAsync(request.ProjectId)
                ?? throw new DoctoralManagementException($"Doctoral project with id {request.ProjectId} not found.", HttpStatusCode.NotFound);

            if (project.Status != ProjectStatus.Submitted && project.Status != ProjectStatus.UnderReview && project.Status != ProjectStatus.ChangesRequested)
            {
                throw new DoctoralManagementException("Only projects in status: Submitted, UnderReview, ChangesRequested can be reviewed.", HttpStatusCode.BadRequest);
            }

            if (!IsValidTransition(project.Status, request.NewStatus))
            {
                throw new DoctoralManagementException($"Invalid status transition from {project.Status} to {request.NewStatus}.", HttpStatusCode.BadRequest);
            }

            var proposalDoc = project.Documents?.FirstOrDefault(d => d.DocumentType == ActivityDocumentType.DoctoralProjectReport);
            if (proposalDoc == null)
            {
                throw new DoctoralManagementException("Doctoral project proposal document is missing.", HttpStatusCode.BadRequest);
            }

            if (request.NewStatus == ProjectStatus.Approved && proposalDoc.Status != DocumentStatus.Approved)
            {
                throw new DoctoralManagementException("Cannot approve project — proposal document must be approved first.", HttpStatusCode.BadRequest);
            }

            if (request.DocumentStatus.HasValue)
            {
                proposalDoc.Status = request.DocumentStatus.Value;
                proposalDoc.ReviewComment = request.ReviewComment;
                proposalDoc.ReviewedAt = DateTime.UtcNow;
                proposalDoc.ReviewedBy = currentUserId;
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
                    ectsTracking.IndependentResearchProject += 14;
                    await _ectsTrackingRepository.UpdateAsync(ectsTracking);

                    await _ectsProgressService.UpdateStudentSemesterAsync(project.StudentId, ectsTracking.TotalECTS);
                }
                else
                {
                    throw new DoctoralManagementException("ECTS tracking record not found for this student", HttpStatusCode.NotFound);
                }
            }

            await _projectRepository.UpdateAsync(project);

            _logger.LogInformation(
                "Doctoral project {ProjectId} reviewed by {Role} {UserId}. New status: {Status}",
                project.Id, currentUserRole, currentUserId, request.NewStatus);

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
                [ProjectStatus.UnderReview] = new() { ProjectStatus.UnderReview, ProjectStatus.Approved, ProjectStatus.ChangesRequested, ProjectStatus.Rejected },
                [ProjectStatus.ChangesRequested] = new() { ProjectStatus.UnderReview, ProjectStatus.Approved, ProjectStatus.Rejected }
            };

            return valid.ContainsKey(current) && valid[current].Contains(next);
        }
    }
}
