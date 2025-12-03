using DoctoralManagement.Application.Dtos;
using MediatR;

namespace DoctoralManagement.Application.ConferenceParticipations.Commands
{
    public class ReviewConferenceCommand : IRequest<ReviewConferenceResponse>
    {
        public int ConferenceId { get; set; }
        public string ReviewComments { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public int EctsAwarded { get; set; }
    }

    public class ReviewConferenceResponse
    {
        public int ConferenceId { get; set; }
        public bool IsApproved { get; set; }
        public ActivityDocumentDto Document { get; set; } = new ActivityDocumentDto();
        public int EctsAwarded { get; set; }
    }
}
