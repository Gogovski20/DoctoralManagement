using MediatR;

namespace DoctoralManagement.Application.DoctoralProjects.Commands
{
    public class SubmitDoctoralProjectCommand : IRequest<SubmitDoctoralProjectResponse>
    {
        public int ProjectId { get; set; }
    }
}
