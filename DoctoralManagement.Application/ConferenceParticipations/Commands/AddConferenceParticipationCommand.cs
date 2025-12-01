using MediatR;

namespace DoctoralManagement.Application.ConferenceParticipations.Commands
{
    public class AddConferenceParticipationCommand : IRequest<AddConferenceParticipationResponse>
    {
        public int StudentId { get; set; }
        public string ConferenceName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Role { get; set; } = string.Empty; 
        public bool IsInternational { get; set; }
        public int PossibleEctsCredits { get; set; }
    }

    public class AddConferenceParticipationResponse
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string ConferenceName { get; set; } = string.Empty;
        public int PossibleEctsCredits { get; set; }
    }
}
