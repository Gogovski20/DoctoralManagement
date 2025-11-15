using MediatR;

namespace DoctoralManagement.Application.DoctoralProjects.Commands
{
    public class DeleteDoctoralProjectCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
