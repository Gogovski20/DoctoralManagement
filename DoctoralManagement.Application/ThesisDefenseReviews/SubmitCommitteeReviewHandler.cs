using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.ThesisDefenseReviews
{
    public class SubmitCommitteeReviewHandler : IRequestHandler<SubmitCommitteeReviewCommand, CommitteeReviewResponse>
    {
        private readonly IThesisDefenseRepository _thesisDefenseRepository;
        private readonly ICommitteeReviewRepository _committeeReviewRepository;

        public SubmitCommitteeReviewHandler(IThesisDefenseRepository thesisDefenseRepository, ICommitteeReviewRepository committeeReviewRepository)
        {
            _thesisDefenseRepository = thesisDefenseRepository;
            _committeeReviewRepository = committeeReviewRepository;
        }

        public async Task<CommitteeReviewResponse> Handle(SubmitCommitteeReviewCommand request, CancellationToken cancellationToken)
        {
            var defense = await _thesisDefenseRepository.GetByIdAsync(request.DefenseId)
                ?? throw new Exception("Defense not found");

            if (defense.Status != Domain.Entities.DefenseStatus.Scheduled)
            {
                throw new Exception("Defense must be scheduled to accept reviews");
            }

            if (!defense.CommitteeMemberIds.Contains(request.ReviewerId))
            {
                throw new Exception("Reviewer is not part of the committee");
            }

            var existingReview = await _committeeReviewRepository
                .GetByDefenseAndReviewerAsync(defense.Id, request.ReviewerId);

            if (existingReview == null)
            {
                existingReview = new Domain.Entities.CommitteeReview
                {
                    ThesisDefenseId = defense.Id,
                    ReviewerId = request.ReviewerId,
                    Comments = request.Comments,
                    ApprovalStatus = request.ApprovalStatus,
                    ReviewedAt = DateTime.UtcNow
                };

                await _committeeReviewRepository.AddAsync(existingReview);
            }
            else
            {
                existingReview.Comments = request.Comments;
                existingReview.ApprovalStatus = request.ApprovalStatus;
                existingReview.ReviewedAt = DateTime.UtcNow;

                await _committeeReviewRepository.UpdateAsync(existingReview);
            }

            return new CommitteeReviewResponse
            {
                Id = existingReview.Id,
                ThesisDefenseId = existingReview.ThesisDefenseId,
                ReviewerId = existingReview.ReviewerId,
                Comments = existingReview.Comments,
                ApprovalStatus = existingReview.ApprovalStatus.ToString(),
                ReviewedAt = existingReview.ReviewedAt
            };
        }
    }
}
