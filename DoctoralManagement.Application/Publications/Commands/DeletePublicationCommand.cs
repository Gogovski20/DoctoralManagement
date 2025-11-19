using MediatR;

namespace DoctoralManagement.Application.Publications.Commands
{
    public class DeletePublicationCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
