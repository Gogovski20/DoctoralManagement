using DoctoralManagement.Application.Dtos;
using MediatR;

namespace DoctoralManagement.Application.Mobilities.Queries
{
    public class GetMobilityByIdQuery : IRequest<GetMobilityByIdResponse>
    {
        public int MobilityId { get; set; }
    }

    public class GetMobilityByIdResponse
    {
        public int Id { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsApproved { get; set; }
        public ActivityDocumentDto? Document { get; set; }
    }
}
