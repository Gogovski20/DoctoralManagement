using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.ConferenceParticipations.Commands
{
    public class UpdateConferenceParticipationHandler : IRequestHandler<UpdateConferenceParticipationCommand, UpdateConferenceParticipationResponse>
    {
        private readonly IConferenceParticipationRepository _conferenceParticipationRepository;
        private readonly IEctsTrackingRepository _ectsTrackingRepository;

        public UpdateConferenceParticipationHandler(IConferenceParticipationRepository conferenceParticipationRepository, IEctsTrackingRepository ectsTrackingRepository)
        {
            _conferenceParticipationRepository = conferenceParticipationRepository;
            _ectsTrackingRepository = ectsTrackingRepository;
        }

        public async Task<UpdateConferenceParticipationResponse> Handle(UpdateConferenceParticipationCommand request, CancellationToken cancellationToken)
        {
            var conference = await _conferenceParticipationRepository.GetByIdAsync(request.Id)
                ?? throw new Exception($"Conference with id {request.Id} not found");

            int oldEcts = conference.EctsAwarded;

            conference.ConferenceName = request.ConferenceName;
            conference.Date = request.Date;
            conference.Role = request.Role;
            conference.IsInternational = request.IsInternational;

            conference.EctsAwarded = CalculateEctsForConference(request);

            await _conferenceParticipationRepository.UpdateAsync(conference);

            var ectsTracking = await _ectsTrackingRepository.GetByIdAsync(conference.StudentId);
            if (ectsTracking != null) 
            {
                ectsTracking.TeachingActivities = ectsTracking.TeachingActivities - oldEcts + conference.EctsAwarded;
                if (ectsTracking.TeachingActivities > 18) 
                {
                    ectsTracking.TeachingActivities = 18;
                }
                await _ectsTrackingRepository.UpdateAsync(ectsTracking);
            }
            return new UpdateConferenceParticipationResponse { };
        }

        private int CalculateEctsForConference(UpdateConferenceParticipationCommand request)
        {
            return request.IsInternational ? 3 : 1;
        }
    }
}
