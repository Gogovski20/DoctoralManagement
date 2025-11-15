using MediatR;

namespace DoctoralManagement.Application.DoctoralProjects.Commands
{
    public class CreateDoctoralProjectDraftCommand : IRequest<CreateDoctoralProjectDraftResponse>
    {
        public int StudentId { get; set; }
        public int MentorId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ResearchArea { get; set; } = string.Empty;
        public string? ProposalDocumentPath { get; set; }
    }
}
