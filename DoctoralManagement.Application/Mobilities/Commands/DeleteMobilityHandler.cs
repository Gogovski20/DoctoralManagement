using DoctoralManagement.Application.ECTS.Services;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Mobilities.Commands
{
    public class DeleteMobilityHandler : IRequestHandler<DeleteMobilityCommand, bool>
    {
        private readonly IMobilityRepository _mobilityRepository;
        private readonly IEctsTrackingRepository _ectsRepository;
        private readonly EctsProgressService _ectsProgressService;

        public DeleteMobilityHandler(IMobilityRepository mobilityRepository, IEctsTrackingRepository ectsRepository, EctsProgressService ectsProgressService)
        {
            _mobilityRepository = mobilityRepository;
            _ectsRepository = ectsRepository;
            _ectsProgressService = ectsProgressService;
        }

        public async Task<bool> Handle(DeleteMobilityCommand request, CancellationToken cancellationToken)
        {
            var mobility = await _mobilityRepository.GetByIdAsync(request.Id);
            if (mobility == null)
                throw new Exception($"Mobility with ID {request.Id} not found.");

            int ectsPoints = CalculateEctsForMobility(mobility.StartDate, mobility.EndDate);
            int studentId = mobility.StudentId;

            await _mobilityRepository.DeleteAsync(request.Id);

            // Update ECTS tracking
            var ectsTracking = await _ectsRepository.GetByStudentIdAsync(mobility.StudentId);
            if (ectsTracking != null)
            {
                ectsTracking.InternationalMobility -= ectsPoints;
                if (ectsTracking.InternationalMobility < 0)
                    ectsTracking.InternationalMobility = 0;

                await _ectsRepository.UpdateAsync(ectsTracking);
                await _ectsProgressService.UpdateStudentSemesterAsync(studentId, ectsTracking.TotalECTS);
            }

            return true;
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
