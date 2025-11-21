using DoctoralManagement.Application.ECTS.Services;
using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Mobilities.Commands
{
    public class AddMobilityHandler : IRequestHandler<AddMobilityCommand, AddMobilityResponse>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IMobilityRepository _mobilityRepository;
        private readonly IEctsTrackingRepository _ectsRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly EctsProgressService _ectsProgressService;

        public AddMobilityHandler(
            IStudentRepository studentRepository,
            IMobilityRepository mobilityRepository,
            IEctsTrackingRepository ectsRepository,
            IApplicationRepository applicationRepository,
            EctsProgressService ectsProgressService)
        {
            _studentRepository = studentRepository;
            _mobilityRepository = mobilityRepository;
            _ectsRepository = ectsRepository;
            _applicationRepository = applicationRepository;
            _ectsProgressService = ectsProgressService;
        }

        public async Task<AddMobilityResponse> Handle(AddMobilityCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId)
                ?? throw new Exception($"Student with id {request.StudentId} not found");

            var hasAccepted = await _applicationRepository.HasFinalAcceptedApplicationAsync(student.Id);
            if (!hasAccepted)
            {
                throw new Exception("Student is not accepted to a doctoral program");
            }


            int ectsPoints = CalculateEctsForMobility(request.StartDate, request.EndDate);

            var mobility = new Mobility
            {
                StudentId = request.StudentId,
                Institution = request.Institution,
                Country = request.Country,
                StartDate = request.StartDate,
                EndDate = request.EndDate
            };

            var created = await _mobilityRepository.AddAsync(mobility);

            var ectsTracking = await _ectsRepository.GetByStudentIdAsync(request.StudentId);
            if (ectsTracking != null)
            {
                ectsTracking.InternationalMobility += ectsPoints;
                if (ectsTracking.InternationalMobility > 6)
                {
                    ectsTracking.InternationalMobility = 6;
                }
                await _ectsRepository.UpdateAsync(ectsTracking);
                await _ectsProgressService.UpdateStudentSemesterAsync(request.StudentId, ectsTracking.TotalECTS);
            }

            return new AddMobilityResponse
            {
                Id = created.Id,
                StudentId = created.StudentId,
                EctsAwarded = ectsPoints
            };
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
