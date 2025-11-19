using MediatR;

namespace DoctoralManagement.Application.Mobilities.Commands
{
    public class DeleteMobilityCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
