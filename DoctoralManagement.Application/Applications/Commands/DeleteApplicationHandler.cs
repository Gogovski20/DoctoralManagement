using DoctoralManagement.Application.Common;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using System.Net;

namespace DoctoralManagement.Application.Applications.Commands
{
    public class DeleteApplicationHandler : IRequestHandler<DeleteApplicationCommand, DeleteApplicationResponse>
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;

        public DeleteApplicationHandler(IApplicationRepository applicationRepository, ICurrentUserService currentUserService, IAuthService authService)
        {
            _applicationRepository = applicationRepository;
            _currentUserService = currentUserService;
            _authService = authService;
        }

        public async Task<DeleteApplicationResponse> Handle(DeleteApplicationCommand request, CancellationToken cancellationToken)
        {
            var application = await _applicationRepository.GetByIdAsync(request.Id);

            if (application == null)
            {
                throw new DoctoralManagementException($"Application with ID {request.Id} not found.", HttpStatusCode.NotFound);
            }

            var currentUserId = _currentUserService.UserId;
            var currentUserRole = _currentUserService.Role;

            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);

            bool isOwner = linkedStudentId == application.StudentId;
            bool isPrivileged = currentUserRole is "Admin";

            if (!isOwner && !isPrivileged)
            {
                throw new DoctoralManagementException("You are not authorized to delete this application", HttpStatusCode.Forbidden);
            }

            if (application.ApplicationStatus != Domain.Entities.ApplicationStatus.Draft)
            {
                throw new DoctoralManagementException("Only applications in 'Draft' status can be deleted.", HttpStatusCode.BadRequest);
            }

            await _applicationRepository.DeleteAsync(application);

            return new DeleteApplicationResponse();
        }
    }
}
