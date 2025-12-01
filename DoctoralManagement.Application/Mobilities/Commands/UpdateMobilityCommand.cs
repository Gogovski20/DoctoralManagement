using MediatR;

namespace DoctoralManagement.Application.Mobilities.Commands
{
    public class UpdateMobilityCommand : IRequest<PublicationResponse>
    {
        public int Id { get; set; }
        public string Institution { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int EctsCredits { get; set; }
    }

    public class PublicationResponse { }
}
