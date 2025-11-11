namespace DoctoralManagement.Domain.Entities
{
    public enum ProjectStatus
    {
        Draft,
        Submitted,
        UnderReview,
        Approved,
        ChangesRequested,
        Rejected
    }

    public class DoctoralProject
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ResearchArea { get; set; } = string.Empty;
        public int EctsCredits { get; set; }

        public int StudentId { get; set; }
        public Student? Student { get; set; } 

        public int MentorId { get; set; }
        public Mentor? Mentor { get; set; }

        public ProjectStatus Status { get; set; } = ProjectStatus.Draft;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? SubmittedAt { get; set; }
        public DateTime? DecisionAt { get; set; }

        public string ProposalDocumentPath { get; set; } = string.Empty;
        public string? CommitteeNotes { get; set; }

        // Navigation
        public ICollection<ThesisDefense> Defenses { get; set; } = new List<ThesisDefense>();
    }
}
