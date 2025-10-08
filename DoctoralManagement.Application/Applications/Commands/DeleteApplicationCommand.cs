using MediatR;

namespace DoctoralManagement.Application.Applications.Commands
{
    public class DeleteApplicationCommand : IRequest<DeleteApplicationResponse>
    {
        public int Id { get; set; }
    }
}
