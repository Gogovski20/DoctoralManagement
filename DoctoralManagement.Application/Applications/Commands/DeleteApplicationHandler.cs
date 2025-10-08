using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Applications.Commands
{
    public class DeleteApplicationHandler : IRequestHandler<DeleteApplicationCommand, DeleteApplicationResponse>
    {
        private readonly IApplicationRepository _applicationRepository;

        public DeleteApplicationHandler(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task<DeleteApplicationResponse> Handle(DeleteApplicationCommand request, CancellationToken cancellationToken)
        {
            var application = await _applicationRepository.GetByIdAsync(request.Id);

            if (application == null)
            {
                throw new Exception($"Application with ID {request.Id} not found.");
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
