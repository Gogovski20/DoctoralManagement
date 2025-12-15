using DoctoralManagement.Application.Common;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DoctoralManagement.Application.Applications.Commands
{
    public class UpdateApplicationHandler : IRequestHandler<UpdateApplicationCommand, UpdateApplicationResponse>
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;
        private readonly ILogger<UpdateApplicationHandler> _logger;

        public UpdateApplicationHandler(IApplicationRepository applicationRepository, ICurrentUserService currentUserService, IAuthService authService, ILogger<UpdateApplicationHandler> logger)
        {
            _applicationRepository = applicationRepository;
            _currentUserService = currentUserService;
            _authService = authService;
            _logger = logger;
        }

        public async Task<UpdateApplicationResponse> Handle(UpdateApplicationCommand request, CancellationToken cancellationToken)
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
                throw new DoctoralManagementException("You are not authorized to update this application", HttpStatusCode.Forbidden);
            }

            if (application.ApplicationStatus != Domain.Entities.ApplicationStatus.Draft && application.ApplicationStatus != Domain.Entities.ApplicationStatus.Submitted)
            {
                throw new DoctoralManagementException("Only applications in 'Draft' or 'Submitted' status can be updated.", HttpStatusCode.BadRequest);
            }

            application.PrefferedMentorId = request.PreferredMentorId;

            await _applicationRepository.UpdateAsync(application);

            _logger.LogInformation(
                "Application {ApplicationId} updated by {UserRole} {UserId}. Preferred mentor: {MentorId}",
                application.Id, currentUserRole, currentUserId, request.PreferredMentorId);

            return new UpdateApplicationResponse
            {
                Id = application.Id,
                StudentId = application.StudentId,
                DoctoralProgramId = application.DoctoralProgramId,
                PreferredMentorId = application.PrefferedMentorId,
                ApplicationStatus = application.ApplicationStatus,
                ApplicationDate = application.ApplicationDate
            };
        }
    }
}
