using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.ECTS.Services;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using System.Net;

namespace DoctoralManagement.Application.ConferenceParticipations.Commands
{
    public class DeleteConferenceParticipationHandler : IRequestHandler<DeleteConferenceParticipationCommand, bool>
    {
        private readonly IConferenceParticipationRepository _conferenceParticipationRepository;
        private readonly IEctsTrackingRepository _ectsTrackingRepository;
        private readonly EctsProgressService _ectsProgressService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;

        public DeleteConferenceParticipationHandler(IConferenceParticipationRepository conferenceParticipationRepository, IEctsTrackingRepository ectsTrackingRepository, EctsProgressService ectsProgressService, ICurrentUserService currentUserService, IAuthService authService)
        {
            _conferenceParticipationRepository = conferenceParticipationRepository;
            _ectsTrackingRepository = ectsTrackingRepository;
            _ectsProgressService = ectsProgressService;
            _currentUserService = currentUserService;
            _authService = authService;
        }

        public async Task<bool> Handle(DeleteConferenceParticipationCommand request, CancellationToken cancellationToken)
        {
            var conference = await _conferenceParticipationRepository.GetByIdAsync(request.Id)
                ?? throw new Exception($"Conference with id {request.Id} not found");

            var currentUserId = _currentUserService.UserId;

            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);

            if (linkedStudentId == null || linkedStudentId != conference.StudentId)
            {
                throw new DoctoralManagementException("You can only delete conference participation for your own account.", HttpStatusCode.Forbidden);
            }

            int ectsPoints = conference.EctsAwarded;
            int studentId = conference.StudentId;

            await _conferenceParticipationRepository.DeleteAsync(conference.Id);

            var ectsTracking = await _ectsTrackingRepository.GetByStudentIdAsync(conference.StudentId);
            if (ectsTracking != null) 
            {
                ectsTracking.TeachingActivities -= ectsPoints;
                if (ectsTracking.TeachingActivities < 0)
                {
                    ectsTracking.TeachingActivities = 0;
                }
                await _ectsTrackingRepository.UpdateAsync(ectsTracking);
                await _ectsProgressService.UpdateStudentSemesterAsync(studentId, ectsTracking.TotalECTS); 
            }
            return true;
        }
    }
}
