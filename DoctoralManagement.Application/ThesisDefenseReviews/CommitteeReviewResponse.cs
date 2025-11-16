namespace DoctoralManagement.Application.ThesisDefenseReviews
{
    public class CommitteeReviewResponse
    {
        public int Id { get; set; }
        public int ThesisDefenseId { get; set; }
        public int ReviewerId { get; set; }
        public string Comments { get; set; } = string.Empty;
        public string ApprovalStatus { get; set; } = string.Empty;
        public DateTime? ReviewedAt { get; set; }
    }
}
