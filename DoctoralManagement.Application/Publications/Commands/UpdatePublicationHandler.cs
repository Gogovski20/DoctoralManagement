using DoctoralManagement.Application.ECTS.Services;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Publications.Commands
{
    public class UpdatePublicationHandler : IRequestHandler<UpdatePublicationCommand, PublicationResponse>
    {
        private readonly IPublicationRepository _publicationRepository;
        private readonly IEctsTrackingRepository _ectsRepository;
        private readonly EctsProgressService _progressService;

        public UpdatePublicationHandler(IPublicationRepository publicationRepository, IEctsTrackingRepository ectsRepository, EctsProgressService progressService)
        {
            _publicationRepository = publicationRepository;
            _ectsRepository = ectsRepository;
            _progressService = progressService;
        }

        public async Task<PublicationResponse> Handle(UpdatePublicationCommand request, CancellationToken cancellationToken)
        {
            var publication = await _publicationRepository.GetByIdAsync(request.Id)
                ?? throw new Exception($"Publication with id {request.Id} not found");

            int oldEcts = publication.EctsPoints;

            publication.Title = request.Title;
            publication.Journal = request.Journal;
            publication.PublishedOn = request.PublishedOn;
            publication.Doi = request.Doi;
            publication.IsIndexedInScopus = request.IsIndexedInScopus;
            publication.IsIndexedInThomsonReuters = request.IsIndexedInThomsonReuters;

            publication.EctsPoints = CalculateEctsForPublication(request.IsIndexedInScopus, request.IsIndexedInThomsonReuters);

            await _publicationRepository.UpdateAsync(publication);

            var ectsTracking = await _ectsRepository.GetByStudentIdAsync(publication.StudentId);
            if (ectsTracking != null)
            {
                ectsTracking.Publications = ectsTracking.Publications - oldEcts + publication.EctsPoints;
                if (ectsTracking.Publications > 27)
                    ectsTracking.Publications = 27;

                await _ectsRepository.UpdateAsync(ectsTracking);
                await _progressService.UpdateStudentSemesterAsync(publication.StudentId, ectsTracking.TotalECTS);
            }

            return new PublicationResponse { };
        }

        private int CalculateEctsForPublication(bool isScopus, bool isThomson)
        {
            if (isScopus || isThomson)
                return 7;
            return 3;
        }
    }
}
