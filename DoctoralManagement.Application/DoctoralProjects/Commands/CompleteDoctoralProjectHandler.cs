using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.ECTS.Services;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DoctoralManagement.Application.DoctoralProjects.Commands
{
    public class CompleteDoctoralProjectHandler : IRequestHandler<CompleteDoctoralProjectCommand, CompleteDoctoralProjectResponse>
    {
        private readonly IDoctoralProjectRepository _projectRepo;
        private readonly IEctsTrackingRepository _ectsRepo;
        private readonly EctsProgressService _ectsProgressService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;
        private readonly ILogger<CompleteDoctoralProjectHandler> _logger;

        public CompleteDoctoralProjectHandler(IDoctoralProjectRepository projectRepo, IEctsTrackingRepository ectsRepo, EctsProgressService ectsProgressService, ICurrentUserService currentUserService, IAuthService authService, ILogger<CompleteDoctoralProjectHandler> logger)
        {
            _projectRepo = projectRepo;
            _ectsRepo = ectsRepo;
            _ectsProgressService = ectsProgressService;
            _currentUserService = currentUserService;
            _authService = authService;
            _logger = logger;
        }

        public async Task<CompleteDoctoralProjectResponse> Handle(CompleteDoctoralProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectRepo.GetByIdAsync(request.ProjectId)
                ?? throw new DoctoralManagementException("Doctoral project not found.", HttpStatusCode.NotFound);

            var currentUserId = _currentUserService.UserId;
            var currentUserRole = _currentUserService.Role;
            var isAdmin = currentUserRole == "Admin";

            int? linkedMentorId = null;
            if (!isAdmin)
            {
                linkedMentorId = await _authService.GetLinkedMentorIdAsync(currentUserId);
            }

            if (!isAdmin && (linkedMentorId == null || project.MentorId != linkedMentorId))
            {
                throw new DoctoralManagementException(
                    "Only the project mentor or an admin can complete this project.",
                    HttpStatusCode.Forbidden);
            }

            if (project.Status != Domain.Entities.ProjectStatus.Approved && project.Status != Domain.Entities.ProjectStatus.UnderReview)
            {
                throw new DoctoralManagementException("Only approved or under review projects can be completed.", HttpStatusCode.BadRequest);
            }

            project.Status = Domain.Entities.ProjectStatus.Completed;
            project.DecisionAt = DateTime.UtcNow;
            await _projectRepo.UpdateAsync(project);

            var ects = await _ectsRepo.GetByStudentIdAsync(project.StudentId);
            if (ects != null)
            {
                ects.IndependentResearchProject += 27;
                if (ects.IndependentResearchProject > 41)
                {
                    ects.IndependentResearchProject = 41;
                }
                await _ectsRepo.UpdateAsync(ects);
                await _ectsProgressService.UpdateStudentSemesterAsync(project.StudentId, ects.TotalECTS);
            }

            _logger.LogInformation(
               "Doctoral project {ProjectId} completed by {Role} {UserId}. Student: {StudentId}",
               project.Id, currentUserRole, currentUserId, project.StudentId);

            return new CompleteDoctoralProjectResponse
            {
                ProjectId = project.Id,
                Status = project.Status.ToString(),
                CompletedAt = project.DecisionAt.Value
            };
        }
    }
}
