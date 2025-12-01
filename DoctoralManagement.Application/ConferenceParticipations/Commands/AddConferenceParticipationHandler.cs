using DoctoralManagement.Application.ECTS.Services;
using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.ConferenceParticipations.Commands
{
    public class AddConferenceParticipationHandler : IRequestHandler<AddConferenceParticipationCommand, AddConferenceParticipationResponse>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IConferenceParticipationRepository _conferenceParticipationRepository;
        private readonly IEctsTrackingRepository _ectsTrackingRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly EctsProgressService _ectsProgressService;

        public AddConferenceParticipationHandler(IStudentRepository studentRepository, IConferenceParticipationRepository conferenceParticipationRepository, IEctsTrackingRepository ectsTrackingRepository, IApplicationRepository applicationRepository, EctsProgressService ectsProgressService)
        {
            _studentRepository = studentRepository;
            _conferenceParticipationRepository = conferenceParticipationRepository;
            _ectsTrackingRepository = ectsTrackingRepository;
            _applicationRepository = applicationRepository;
            _ectsProgressService = ectsProgressService;
        }

        public async Task<AddConferenceParticipationResponse> Handle(AddConferenceParticipationCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId)
                ?? throw new Exception($"Student with id {request.StudentId} not found");

            var hasFinalAccepted = await _applicationRepository.HasFinalAcceptedApplicationAsync(student.Id);
            if (!hasFinalAccepted)
            {
                throw new Exception("Student is not accepted to a doctoral program");
            }

            var participation = new ConferenceParticipation 
            {
                StudentId = request.StudentId,
                ConferenceName = request.ConferenceName,
                Date = request.Date,
                Role = request.Role,
                IsInternational = request.IsInternational,
                EctsAwarded = request.PossibleEctsCredits
            };

            var created = await _conferenceParticipationRepository.AddAsync(participation);

            //var ectsTracking = await _ectsTrackingRepository.GetByStudentIdAsync(student.Id);
            //if (ectsTracking != null)
            //{
            //    ectsTracking.TeachingActivities += ectsPoints;
            //    if (ectsTracking.TeachingActivities > 18)
            //    {
            //        ectsTracking.TeachingActivities = 18;
            //    }
            //    await _ectsTrackingRepository.UpdateAsync(ectsTracking);
            //    await _ectsProgressService.UpdateStudentSemesterAsync(request.StudentId, ectsTracking.TotalECTS);
            //}

            return new AddConferenceParticipationResponse 
            {
                Id = created.Id,
                StudentId = created.StudentId,
                ConferenceName = created.ConferenceName,
                PossibleEctsCredits = created.EctsAwarded
            };
        }       
    }
}
