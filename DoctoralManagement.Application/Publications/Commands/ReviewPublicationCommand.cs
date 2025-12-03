using DoctoralManagement.Application.Dtos;
using MediatR;

namespace DoctoralManagement.Application.Publications.Commands
{
    public class ReviewPublicationCommand : IRequest<ReviewPublicationResponse>
    {
        public int PublicationId { get; set; }
        public string ReviewComments { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public int EctsAwarded { get; set; }
    }

    public class ReviewPublicationResponse
    {
        public int PublicationId { get; set; }
        public bool IsApproved { get; set; }
        public ActivityDocumentDto Document { get; set; } = new ActivityDocumentDto();
        public int EctsAwarded { get; set; }
    }
}
