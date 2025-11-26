using DoctoralManagement.Application.Common;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Applications.Commands
{
    public class DeleteApplicationHandler : IRequestHandler<DeleteApplicationCommand, DeleteApplicationResponse>
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly ICurrentUserService _currentUserService;


        public DeleteApplicationHandler(IApplicationRepository applicationRepository, ICurrentUserService currentUserService)
        {
            _applicationRepository = applicationRepository;
            _currentUserService = currentUserService;
        }

        public async Task<DeleteApplicationResponse> Handle(DeleteApplicationCommand request, CancellationToken cancellationToken)
        {
            var application = await _applicationRepository.GetByIdAsync(request.Id);

            if (application == null)
            {
                throw new Exception($"Application with ID {request.Id} not found.");
            }

            var currentUserId = _currentUserService.UserId;
            var currentUserRole = _currentUserService.Role;

            if (application.StudentId != currentUserId || currentUserRole != "Secretary")
            {
                throw new UnauthorizedAccessException("Not allowed to delete this application");
            }

            if (application.ApplicationStatus != Domain.Entities.ApplicationStatus.Draft)
            {
                throw new Exception("Only applications in 'Draft' status can be deleted.");
            }

            await _applicationRepository.DeleteAsync(application);

            return new DeleteApplicationResponse();
        }
    }
}
