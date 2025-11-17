namespace DoctoralManagement.Application.ThesisDefenseReviews
{
    public class FinalizeCommitteeReviewsResponse
    {
        public int DefenseId { get; set; }
        public string FinalDecision { get; set; } = string.Empty;

        public List<CommitteeReviewResponse> Reviews { get; set; } = new();
    }
}
