namespace DoctoralManagement.Domain.Entities
{
    public enum  DefenseStatus
    {
        Scheduled,
        Completed,
        Failed,
        Passed
    }

    public class ThesisDefense
    {
        public int Id { get; set; }

        public int DoctoralProjectId { get; set; }
        public DoctoralProject DoctoralProject { get; set; } = null!;

        public DateTime ScheduledAt { get; set; }
        public string Room { get; set; } = string.Empty;

        public List<int> CommitteeMemberIds { get; set; } = new();

        public DefenseStatus Status { get; set; } = DefenseStatus.Scheduled;
        public string? ResultNotes { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? ArchiveNumber { get; set; }

        public List<CommitteeReview> Reviews { get; set; } = new();
    }
}
