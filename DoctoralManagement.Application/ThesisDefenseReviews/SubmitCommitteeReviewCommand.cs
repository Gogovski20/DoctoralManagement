using DoctoralManagement.Domain.Entities;
using MediatR;

namespace DoctoralManagement.Application.ThesisDefenseReviews
{
    public class SubmitCommitteeReviewCommand : IRequest<CommitteeReviewResponse>
    {
        public int DefenseId { get; set; }
        public int ReviewerId { get; set; }
        public string Comments { get; set; } = string.Empty;
        public CommitteeApprovalStatus ApprovalStatus { get; set; }
    }
}
