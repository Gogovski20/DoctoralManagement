namespace DoctoralManagement.Application.DoctoralProjects.Commands
{
    public class UpdateDoctoralProjectResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ResearchArea { get; set; } = string.Empty;
        public int EctsCredits { get; set; }
        public string Status { get; set; } = string.Empty;
        public int MentorId { get; set; }
        public string? ProposalDocumentPath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
    }
}
