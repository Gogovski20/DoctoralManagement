using DoctoralManagement.Application.Common;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DoctoralManagement.Application.DoctoralProjects.Commands
{
    public class DeleteDoctoralProjectHandler : IRequestHandler<DeleteDoctoralProjectCommand, bool>
    {
        private readonly IDoctoralProjectRepository _repository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;
        private readonly ILogger<DeleteDoctoralProjectHandler> _logger;

        public DeleteDoctoralProjectHandler(IDoctoralProjectRepository repository, ICurrentUserService currentUserService, IAuthService authService, ILogger<DeleteDoctoralProjectHandler> logger)
        {
            _repository = repository;
            _currentUserService = currentUserService;
            _authService = authService;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteDoctoralProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _repository.GetByIdAsync(request.Id)
                ?? throw new DoctoralManagementException($"Doctoral project with id {request.Id} not found.", HttpStatusCode.NotFound);

            var currentUserId = _currentUserService.UserId;
            var currentUserRole = _currentUserService.Role;
            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);
            var isAdmin = currentUserRole == "Admin";

            if (!isAdmin && (linkedStudentId == null || linkedStudentId != project.StudentId))
            {
                throw new DoctoralManagementException(
                    "You can only delete your own doctoral projects.",
                    HttpStatusCode.Forbidden);
            }

            if (project.Status != Domain.Entities.ProjectStatus.Draft)
            {
                throw new DoctoralManagementException("Only projects in 'Draft' status can be deleted.", HttpStatusCode.BadRequest);
            }

            await _repository.DeleteAsync(request.Id);

            _logger.LogInformation(
                "Doctoral project {ProjectId} deleted by {Role} {UserId}. Student: {StudentId}",
                project.Id, currentUserRole, currentUserId, project.StudentId);

            return true;
        }
    }
}
