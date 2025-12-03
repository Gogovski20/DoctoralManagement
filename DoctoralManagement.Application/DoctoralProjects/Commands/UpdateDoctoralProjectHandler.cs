using DoctoralManagement.Application.Common;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DoctoralManagement.Application.DoctoralProjects.Commands
{
    public class UpdateDoctoralProjectHandler : IRequestHandler<UpdateDoctoralProjectCommand, UpdateDoctoralProjectResponse>
    {
        private readonly IDoctoralProjectRepository _doctoralProjectRepository;
        private readonly IMentorRepository _mentorRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;
        private readonly ILogger<UpdateDoctoralProjectHandler> _logger;

        public UpdateDoctoralProjectHandler(IDoctoralProjectRepository doctoralProjectRepository, IMentorRepository mentorRepository, ICurrentUserService currentUserService, IAuthService authService, ILogger<UpdateDoctoralProjectHandler> logger)
        {
            _doctoralProjectRepository = doctoralProjectRepository;
            _mentorRepository = mentorRepository;
            _currentUserService = currentUserService;
            _authService = authService;
            _logger = logger;
        }

        public async Task<UpdateDoctoralProjectResponse> Handle(UpdateDoctoralProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _doctoralProjectRepository.GetByIdAsync(request.Id)
                ?? throw new DoctoralManagementException($"Doctoral project with id {request.Id} not found.", HttpStatusCode.NotFound);

            var currentUserId = _currentUserService.UserId;
            var currentUserRole = _currentUserService.Role;
            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);
            var isAdmin = currentUserRole == "Admin";

            if (!isAdmin && (linkedStudentId == null || linkedStudentId != project.StudentId))
            {
                throw new DoctoralManagementException(
                    "You can only update your own doctoral projects.",
                    HttpStatusCode.Forbidden);
            }


            if (project.Status != Domain.Entities.ProjectStatus.Draft && project.Status != Domain.Entities.ProjectStatus.ChangesRequested)
            {
                throw new DoctoralManagementException("Only projects in Draft or ChangesRequested status can be updated.", HttpStatusCode.BadRequest);
            }

            var mentor = await _mentorRepository.GetByIdAsync(request.MentorId)
                ?? throw new DoctoralManagementException($"Mentor with id {request.MentorId} not found.", HttpStatusCode.NotFound);

            project.Title = request.Title;
            project.ResearchArea = request.ResearchArea;
            project.EctsCredits = request.EctsCredits;
            project.MentorId = request.MentorId;

            await _doctoralProjectRepository.UpdateAsync(project);

            _logger.LogInformation(
                "Doctoral project {ProjectId} updated by {Role} {UserId}. New mentor: {MentorId}",
                project.Id, currentUserRole, currentUserId, request.MentorId);

            return new UpdateDoctoralProjectResponse
            {
                Id = project.Id,
                Title = project.Title,
                ResearchArea = project.ResearchArea,
                EctsCredits = project.EctsCredits,
                Status = project.Status.ToString(),
                MentorId = project.MentorId,
                CreatedAt = project.CreatedAt,
                SubmittedAt = project.SubmittedAt
            };
        }
    }
}
