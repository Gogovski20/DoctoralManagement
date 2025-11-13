using DoctoralManagement.Domain.Entities;
using MediatR;

namespace DoctoralManagement.Application.DoctoralProjects.Commands
{
    public class ReviewDoctoralProjectCommand : IRequest<ReviewDoctoralProjectResponse>
    {
        public int ProjectId { get; set; }
        public ProjectStatus NewStatus { get; set; }
        public string? CommitteeNotes { get; set; }
    }
}
