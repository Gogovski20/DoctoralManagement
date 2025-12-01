using MediatR;

namespace DoctoralManagement.Application.DoctoralProjects.Commands
{
    public class CompleteDoctoralProjectCommand : IRequest<CompleteDoctoralProjectResponse>
    {
        public int ProjectId { get; set; }
        public string? FinalReportNotes { get; set; }
    }

    public class CompleteDoctoralProjectResponse
    {
        public int ProjectId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CompletedAt { get; set; }
    }
}
