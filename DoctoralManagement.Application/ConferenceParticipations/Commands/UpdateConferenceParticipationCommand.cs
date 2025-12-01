using MediatR;

namespace DoctoralManagement.Application.ConferenceParticipations.Commands
{
    public class UpdateConferenceParticipationCommand : IRequest<UpdateConferenceParticipationResponse>
    {
        public int Id { get; set; }
        public string ConferenceName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool IsInternational { get; set; }
        public int EctsCredits { get; set; }

    }

    public class UpdateConferenceParticipationResponse { }
}
