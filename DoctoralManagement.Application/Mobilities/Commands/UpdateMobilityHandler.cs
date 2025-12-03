using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.ECTS.Services;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using System.Net;

namespace DoctoralManagement.Application.Mobilities.Commands
{
    public class UpdateMobilityHandler : IRequestHandler<UpdateMobilityCommand, PublicationResponse>
    {
        private readonly IMobilityRepository _mobilityRepository;
        private readonly IEctsTrackingRepository _ectsRepository;
        private readonly EctsProgressService _progressService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;

        public UpdateMobilityHandler(IMobilityRepository mobilityRepository, IEctsTrackingRepository ectsRepository, EctsProgressService progressService, ICurrentUserService currentUserService, IAuthService authService)
        {
            _mobilityRepository = mobilityRepository;
            _ectsRepository = ectsRepository;
            _progressService = progressService;
            _currentUserService = currentUserService;
            _authService = authService;
        }

        public async Task<PublicationResponse> Handle(UpdateMobilityCommand request, CancellationToken cancellationToken)
        {
            var mobility = await _mobilityRepository.GetByIdAsync(request.Id);
            if (mobility == null)
                throw new DoctoralManagementException($"Mobility with ID {request.Id} not found.", HttpStatusCode.NotFound);

            var currentUserId = _currentUserService.UserId;
            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);

            if (linkedStudentId == null || linkedStudentId != mobility.StudentId)
            {
                throw new DoctoralManagementException(
                    "You can only update your own mobilities.",
                    HttpStatusCode.Forbidden);
            }

            if (mobility.IsApproved)
            {
                throw new DoctoralManagementException(
                    "Cannot update an already approved mobility.",
                    HttpStatusCode.BadRequest);
            }

            int oldEcts = mobility.EctsPoints;

            mobility.Institution = request.Institution;
            mobility.Country = request.Country;
            mobility.StartDate = request.StartDate;
            mobility.EndDate = request.EndDate;

            mobility.EctsPoints = request.EctsCredits;

            await _mobilityRepository.UpdateAsync(mobility);

            

            // Update ECTS tracking
            var ectsTracking = await _ectsRepository.GetByStudentIdAsync(mobility.StudentId);
            if (ectsTracking != null)
            {
                ectsTracking.InternationalMobility = ectsTracking.InternationalMobility - oldEcts + mobility.EctsPoints;
                if (ectsTracking.InternationalMobility > 6)
                    ectsTracking.InternationalMobility = 6;
                else if (ectsTracking.InternationalMobility < 0)
                    ectsTracking.InternationalMobility = 0;

                await _ectsRepository.UpdateAsync(ectsTracking);
                await _progressService.UpdateStudentSemesterAsync(mobility.StudentId, ectsTracking.TotalECTS);
            }

            return new PublicationResponse { };
        }
    }
}
