namespace DoctoralManagement.Application.DoctoralProjects.Commands
{
    public class CreateDoctoralProjectDraftResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ResearchArea { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? ProposalDocumentPath { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
