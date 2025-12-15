using MediatR;

namespace DoctoralManagement.Application.Mobilities.Commands
{
    public class UpdateMobilityCommand : IRequest<MobilityResponse>
    {
        public int Id { get; set; }
        public string Institution { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class MobilityResponse { }
}
