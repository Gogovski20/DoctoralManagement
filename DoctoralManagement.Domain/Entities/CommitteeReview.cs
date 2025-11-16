namespace DoctoralManagement.Domain.Entities
{
    public enum CommitteeApprovalStatus
    {
        Pending,
        Approved,
        ChangesRequired,
        Rejected
    }

    public class CommitteeReview
    {
        public int Id { get; set; }

        public int ThesisDefenseId { get; set; }
        public ThesisDefense ThesisDefense { get; set; } = null!;

        public int ReviewerId { get; set; }
        public string Comments { get; set; } = string.Empty;

        public CommitteeApprovalStatus ApprovalStatus { get; set; } = CommitteeApprovalStatus.Pending;

        public DateTime? ReviewedAt { get; set; }
    }
}
