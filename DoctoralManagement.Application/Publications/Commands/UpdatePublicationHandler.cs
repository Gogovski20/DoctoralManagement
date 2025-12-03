using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.ECTS.Services;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using System.Net;

namespace DoctoralManagement.Application.Publications.Commands
{
    public class UpdatePublicationHandler : IRequestHandler<UpdatePublicationCommand, PublicationResponse>
    {
        private readonly IPublicationRepository _publicationRepository;
        private readonly IEctsTrackingRepository _ectsRepository;
        private readonly EctsProgressService _progressService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;

        public UpdatePublicationHandler(IPublicationRepository publicationRepository, IEctsTrackingRepository ectsRepository, EctsProgressService progressService, ICurrentUserService currentUserService, IAuthService authService)
        {
            _publicationRepository = publicationRepository;
            _ectsRepository = ectsRepository;
            _progressService = progressService;
            _currentUserService = currentUserService;
            _authService = authService;
        }

        public async Task<PublicationResponse> Handle(UpdatePublicationCommand request, CancellationToken cancellationToken)
        {
            var publication = await _publicationRepository.GetByIdAsync(request.Id)
                ?? throw new DoctoralManagementException($"Publication with id {request.Id} not found", HttpStatusCode.NotFound);

            var currentUserId = _currentUserService.UserId;
            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);

            if (linkedStudentId == null || linkedStudentId != publication.StudentId)
            {
                throw new DoctoralManagementException(
                    "You can only update your own publications.",
                    HttpStatusCode.Forbidden);
            }

            if (publication.IsApproved)
            {
                throw new DoctoralManagementException(
                    "Cannot update an already approved publication.",
                    HttpStatusCode.BadRequest);
            }

            int oldEcts = publication.EctsPoints;

            publication.Title = request.Title;
            publication.Journal = request.Journal;
            publication.PublishedOn = request.PublishedOn;
            publication.Doi = request.Doi;
            publication.IsIndexedInScopus = request.IsIndexedInScopus;
            publication.IsIndexedInThomsonReuters = request.IsIndexedInThomsonReuters;

            publication.EctsPoints = request.EctsCredits;

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
    }
}
