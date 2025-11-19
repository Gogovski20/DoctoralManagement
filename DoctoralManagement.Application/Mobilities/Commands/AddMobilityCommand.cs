using MediatR;

namespace DoctoralManagement.Application.Mobilities.Commands
{
    public class AddMobilityCommand : IRequest<AddMobilityResponse>
    {
        public int StudentId { get; set; }
        public string Institution { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class AddMobilityResponse
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int EctsAwarded { get; set; }
    }
}
