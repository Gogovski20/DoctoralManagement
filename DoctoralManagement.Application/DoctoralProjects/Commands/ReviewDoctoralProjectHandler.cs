using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.DoctoralProjects.Commands
{
    public class ReviewDoctoralProjectHandler : IRequestHandler<ReviewDoctoralProjectCommand, ReviewDoctoralProjectResponse>
    {
        private readonly IDoctoralProjectRepository _projectRepository;
        private readonly IMentorRepository _mentorRepository;

        public ReviewDoctoralProjectHandler(IDoctoralProjectRepository projectRepository, IMentorRepository mentorRepository)
        {
            _projectRepository = projectRepository;
            _mentorRepository = mentorRepository;
        }

        public async Task<ReviewDoctoralProjectResponse> Handle(ReviewDoctoralProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetByIdAsync(request.ProjectId)
                ?? throw new Exception($"Doctoral project with id {request.ProjectId} not found.");

            if (!IsValidTransition(project.Status, request.NewStatus))
            {
                throw new Exception($"Invalid status transition from {project.Status} to {request.NewStatus}.");
            }

            if (request.NewStatus == ProjectStatus.Approved)
            {
                var mentorAvailable = await _mentorRepository.IsAvailableForNewStudentAsync(project.MentorId);
                if (!mentorAvailable)
                {
                    throw new Exception("Mentor cannot be assigned — reached maximum number of supervised students.");
                }
            }

            project.Status = request.NewStatus;
            project.CommitteeNotes = request.CommitteeNotes;

            if (request.NewStatus == ProjectStatus.Approved ||
                request.NewStatus == ProjectStatus.Rejected)
            {
                project.DecisionAt = DateTime.UtcNow;
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
