using DoctoralManagement.Application.ECTS.Services;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Publications.Commands
{
    public class DeletePublicationHandler : IRequestHandler<DeletePublicationCommand, bool>
    {
        private readonly IPublicationRepository _publicationRepository;
        private readonly IEctsTrackingRepository _ectsRepository;
        private readonly EctsProgressService _progressService;

        public DeletePublicationHandler(IPublicationRepository publicationRepository, IEctsTrackingRepository ectsRepository, EctsProgressService progressService)
        {
            _publicationRepository = publicationRepository;
            _ectsRepository = ectsRepository;
            _progressService = progressService;
        }

        public async Task<bool> Handle(DeletePublicationCommand request, CancellationToken cancellationToken)
        {
            var publication = await _publicationRepository.GetByIdAsync(request.Id);
            if (publication == null)
                throw new Exception($"Publication with ID {request.Id} not found.");

            int ectsPoints = publication.EctsPoints;
            int studentId = publication.StudentId;

            await _publicationRepository.DeleteAsync(request.Id);

            // Update ECTS tracking
            var ectsTracking = await _ectsRepository.GetByStudentIdAsync(publication.StudentId);
            if (ectsTracking != null)
            {
                ectsTracking.Publications -= ectsPoints;
                if (ectsTracking.Publications < 0)
                    ectsTracking.Publications = 0;

                await _ectsRepository.UpdateAsync(ectsTracking);
                await _progressService.UpdateStudentSemesterAsync(studentId, ectsTracking.TotalECTS);
            }

            return true;
        }
    }
}
