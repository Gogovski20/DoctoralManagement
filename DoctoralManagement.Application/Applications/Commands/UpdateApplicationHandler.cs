using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Applications.Commands
{
    public class UpdateApplicationHandler : IRequestHandler<UpdateApplicationCommand, UpdateApplicationResponse>
    {
        private readonly IApplicationRepository _applicationRepository;

        public UpdateApplicationHandler(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task<UpdateApplicationResponse> Handle(UpdateApplicationCommand request, CancellationToken cancellationToken)
        {
            var application = await _applicationRepository.GetByIdAsync(request.Id);

            if (application == null)
            {
                throw new Exception($"Application with ID {request.Id} not found.");
            }

            if (application.ApplicationStatus != Domain.Entities.ApplicationStatus.Draft)
            {
                throw new Exception("Only applications in 'Draft' status can be updated.");
            }

            application.PrefferedMentorId = request.PreferredMentorId;
            application.MotivationLetter = request.MotivationLetter;
            application.ResearchProposal = request.ResearchProposal;
            application.EnglishCertificatePath = request.EnglishCertificatePath;

            await _applicationRepository.UpdateAsync(application);

            return new UpdateApplicationResponse
            {
                Id = application.Id,
                StudentId = application.StudentId,
                DoctoralProgramId = application.DoctoralProgramId,
                PreferredMentorId = application.PrefferedMentorId,
                MotivationLetter = application.MotivationLetter,
                ResearchProposal = application.ResearchProposal,
                EnglishCertificatePath = application.EnglishCertificatePath,
                ApplicationStatus = application.ApplicationStatus,
                ApplicationDate = application.ApplicationDate
            };
        }
    }
}
