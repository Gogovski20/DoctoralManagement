using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.ECTS.Services;
using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using System.Net;

namespace DoctoralManagement.Application.Mobilities.Commands
{
    public class AddMobilityHandler : IRequestHandler<AddMobilityCommand, AddMobilityResponse>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IMobilityRepository _mobilityRepository;
        private readonly IEctsTrackingRepository _ectsRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly EctsProgressService _ectsProgressService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;

        public AddMobilityHandler(
            IStudentRepository studentRepository,
            IMobilityRepository mobilityRepository,
            IEctsTrackingRepository ectsRepository,
            IApplicationRepository applicationRepository,
            EctsProgressService ectsProgressService,
            ICurrentUserService currentUserService,
            IAuthService authService)
        {
            _studentRepository = studentRepository;
            _mobilityRepository = mobilityRepository;
            _ectsRepository = ectsRepository;
            _applicationRepository = applicationRepository;
            _ectsProgressService = ectsProgressService;
            _currentUserService = currentUserService;
            _authService = authService;
        }

        public async Task<AddMobilityResponse> Handle(AddMobilityCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId)
                ?? throw new DoctoralManagementException($"Student with id {request.StudentId} not found", HttpStatusCode.NotFound);

            var currentUserId = _currentUserService.UserId;

            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);

            if (linkedStudentId == null || linkedStudentId != request.StudentId)
            {
                throw new DoctoralManagementException("You can only add mobility for your own account.", HttpStatusCode.Forbidden);
            }

            var hasAccepted = await _applicationRepository.HasFinalAcceptedApplicationAsync(student.Id);
            if (!hasAccepted)
            {
                throw new DoctoralManagementException("Student is not accepted to a doctoral program", HttpStatusCode.BadRequest);
            }

            var startDateUtc = DateTime.SpecifyKind(request.StartDate, DateTimeKind.Utc);
            var endDateUtc = DateTime.SpecifyKind(request.EndDate, DateTimeKind.Utc);

            var mobility = new Mobility
            {
                StudentId = request.StudentId,
                Institution = request.Institution,
                Country = request.Country,
                StartDate = startDateUtc,
                EndDate = endDateUtc,
            };

            var created = await _mobilityRepository.AddAsync(mobility);

            //var ectsTracking = await _ectsRepository.GetByStudentIdAsync(request.StudentId);
            //if (ectsTracking != null)
            //{
            //    ectsTracking.InternationalMobility += ectsPoints;
            //    if (ectsTracking.InternationalMobility > 6)
            //    {
            //        ectsTracking.InternationalMobility = 6;
            //    }
            //    await _ectsRepository.UpdateAsync(ectsTracking);
            //    await _ectsProgressService.UpdateStudentSemesterAsync(request.StudentId, ectsTracking.TotalECTS);
            //}

            return new AddMobilityResponse
            {
                Id = created.Id,
                StudentId = created.StudentId,
            };
        }
    }
}
