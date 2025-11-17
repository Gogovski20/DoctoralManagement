using MediatR;

namespace DoctoralManagement.Application.ThesisDefenseReviews
{
    public class FinalizeCommitteeReviewsCommand : IRequest<FinalizeCommitteeReviewsResponse>
    {
        public int DefenseId { get; set; }
    }
}
