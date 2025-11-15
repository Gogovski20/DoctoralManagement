namespace DoctoralManagement.Application.DoctoralProjects.Queries
{
    public class GetDoctoralProjectResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ResearchArea { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string MentorName { get; set; } = string.Empty;
        public string? ProposalDocumentPath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
    }
}
