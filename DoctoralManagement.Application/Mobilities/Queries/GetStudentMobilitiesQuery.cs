using DoctoralManagement.Application.Dtos;
using MediatR;

namespace DoctoralManagement.Application.Mobilities.Queries
{
    public class GetStudentMobilitiesQuery : IRequest<IEnumerable<MobilityResponse>>
    {
        public int StudentId { get; set; }
    }

    public class MobilityResponse
    {
        public int Id { get; set; }
        public string Institution { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DocumentDto? Document { get; set; }
    }
}
