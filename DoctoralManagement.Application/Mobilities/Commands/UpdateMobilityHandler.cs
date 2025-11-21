using DoctoralManagement.Application.ECTS.Services;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Mobilities.Commands
{
    public class UpdateMobilityHandler : IRequestHandler<UpdateMobilityCommand, PublicationResponse>
    {
        private readonly IMobilityRepository _mobilityRepository;
        private readonly IEctsTrackingRepository _ectsRepository;
        private readonly EctsProgressService _progressService;

        public UpdateMobilityHandler(IMobilityRepository mobilityRepository, IEctsTrackingRepository ectsRepository, EctsProgressService progressService)
        {
            _mobilityRepository = mobilityRepository;
            _ectsRepository = ectsRepository;
            _progressService = progressService;
        }

        public async Task<PublicationResponse> Handle(UpdateMobilityCommand request, CancellationToken cancellationToken)
        {
            var mobility = await _mobilityRepository.GetByIdAsync(request.Id);
            if (mobility == null)
                throw new Exception($"Mobility with ID {request.Id} not found.");

            int oldEcts = CalculateEctsForMobility(mobility.StartDate, mobility.EndDate);

            mobility.Institution = request.Institution;
            mobility.Country = request.Country;
            mobility.StartDate = request.StartDate;
            mobility.EndDate = request.EndDate;

            await _mobilityRepository.UpdateAsync(mobility);

            int newEcts = CalculateEctsForMobility(request.StartDate, request.EndDate);

            // Update ECTS tracking
            var ectsTracking = await _ectsRepository.GetByStudentIdAsync(mobility.StudentId);
            if (ectsTracking != null)
            {
                ectsTracking.InternationalMobility = ectsTracking.InternationalMobility - oldEcts + newEcts;
                if (ectsTracking.InternationalMobility > 6)
                    ectsTracking.InternationalMobility = 6;
                else if (ectsTracking.InternationalMobility < 0)
                    ectsTracking.InternationalMobility = 0;

                await _ectsRepository.UpdateAsync(ectsTracking);
                await _progressService.UpdateStudentSemesterAsync(mobility.StudentId, ectsTracking.TotalECTS);
            }

            return new PublicationResponse { };
        }

        private int CalculateEctsForMobility(DateTime start, DateTime end)
        {
            var totalMonths = (end - start).TotalDays / 30;
            if (totalMonths >= 3)
                return 6;
            else if (totalMonths >= 1)
                return 3;
            else
                return 0;
        }
    }
}
