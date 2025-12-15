using DoctoralManagement.Application.Dtos;
using MediatR;

namespace DoctoralManagement.Application.Mobilities.Queries
{
    public class GetAllMobilitiesQuery : IRequest<IEnumerable<GetAllMobilitiesResponse>>
    {
    }

    public class GetAllMobilitiesResponse
    {
        public int Id { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DocumentDto? Document { get; set; }
    }
}
