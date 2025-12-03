using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.ECTS.Services;
using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using System.Net;

namespace DoctoralManagement.Application.ConferenceParticipations.Commands
{
    public class AddConferenceParticipationHandler : IRequestHandler<AddConferenceParticipationCommand, AddConferenceParticipationResponse>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IConferenceParticipationRepository _conferenceParticipationRepository;
        private readonly IEctsTrackingRepository _ectsTrackingRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly EctsProgressService _ectsProgressService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;

        public AddConferenceParticipationHandler(IStudentRepository studentRepository, IConferenceParticipationRepository conferenceParticipationRepository, IEctsTrackingRepository ectsTrackingRepository, IApplicationRepository applicationRepository, EctsProgressService ectsProgressService, ICurrentUserService currentUserService, IAuthService authService)
        {
            _studentRepository = studentRepository;
            _conferenceParticipationRepository = conferenceParticipationRepository;
            _ectsTrackingRepository = ectsTrackingRepository;
            _applicationRepository = applicationRepository;
            _ectsProgressService = ectsProgressService;
            _currentUserService = currentUserService;
            _authService = authService;
        }

        public async Task<AddConferenceParticipationResponse> Handle(AddConferenceParticipationCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId)
                ?? throw new DoctoralManagementException($"Student with id {request.StudentId} not found", HttpStatusCode.NotFound);

            var currentUserId = _currentUserService.UserId;

            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);

            if (linkedStudentId == null || linkedStudentId != request.StudentId)
            {
                throw new DoctoralManagementException("You can only add conference participation for your own account.", HttpStatusCode.Forbidden);
            }

            var hasFinalAccepted = await _applicationRepository.HasFinalAcceptedApplicationAsync(student.Id);
            if (!hasFinalAccepted)
            {
                throw new DoctoralManagementException("You must be accepted to a doctoral program before registering conference participation.", HttpStatusCode.BadRequest);
            }

            var participation = new ConferenceParticipation 
            {
                StudentId = request.StudentId,
                ConferenceName = request.ConferenceName,
                Date = request.Date,
                Role = request.Role,
                IsInternational = request.IsInternational,
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
            };
        }       
    }
}
