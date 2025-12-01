using DoctoralManagement.Application.Dtos;
using MediatR;

namespace DoctoralManagement.Application.Mobilities.Commands
{
    public class ReviewMobilityCommand : IRequest<ReviewMobilityResponse>
    {
        public int MobilityId { get; set; }
        public string ReviewComments { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
    }

    public class ReviewMobilityResponse
    {
        public int MobilityId { get; set; }
        public bool IsApproved { get; set; }
        public ActivityDocumentDto Document { get; set; } = new ActivityDocumentDto();
    }
}
