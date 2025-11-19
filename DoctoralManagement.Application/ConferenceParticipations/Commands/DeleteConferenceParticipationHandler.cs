using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.ConferenceParticipations.Commands
{
    public class DeleteConferenceParticipationHandler : IRequestHandler<DeleteConferenceParticipationCommand, bool>
    {
        private readonly IConferenceParticipationRepository _conferenceParticipationRepository;
        private readonly IEctsTrackingRepository _ectsTrackingRepository;

        public DeleteConferenceParticipationHandler(IConferenceParticipationRepository conferenceParticipationRepository, IEctsTrackingRepository ectsTrackingRepository)
        {
            _conferenceParticipationRepository = conferenceParticipationRepository;
            _ectsTrackingRepository = ectsTrackingRepository;
        }

        public async Task<bool> Handle(DeleteConferenceParticipationCommand request, CancellationToken cancellationToken)
        {
            var conference = await _conferenceParticipationRepository.GetByIdAsync(request.Id)
                ?? throw new Exception($"Conference with id {request.Id} not found");

            int ectsPoints = conference.EctsAwarded;

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
            }
            return true;
        }
    }
}
