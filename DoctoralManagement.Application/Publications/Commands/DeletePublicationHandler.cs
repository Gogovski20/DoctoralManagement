using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Publications.Commands
{
    public class DeletePublicationHandler : IRequestHandler<DeletePublicationCommand, bool>
    {
        private readonly IPublicationRepository _publicationRepository;
        private readonly IEctsTrackingRepository _ectsRepository;

        public DeletePublicationHandler(IPublicationRepository publicationRepository, IEctsTrackingRepository ectsRepository)
        {
            _publicationRepository = publicationRepository;
            _ectsRepository = ectsRepository;
        }

        public async Task<bool> Handle(DeletePublicationCommand request, CancellationToken cancellationToken)
        {
            var publication = await _publicationRepository.GetByIdAsync(request.Id);
            if (publication == null)
                throw new Exception($"Publication with ID {request.Id} not found.");

            int ectsPoints = publication.EctsPoints;

            await _publicationRepository.DeleteAsync(request.Id);

            // Update ECTS tracking
            var ectsTracking = await _ectsRepository.GetByStudentIdAsync(publication.StudentId);
            if (ectsTracking != null)
            {
                ectsTracking.Publications -= ectsPoints;
                if (ectsTracking.Publications < 0)
                    ectsTracking.Publications = 0;

                await _ectsRepository.UpdateAsync(ectsTracking);
            }

            return true;
        }
    }
}
