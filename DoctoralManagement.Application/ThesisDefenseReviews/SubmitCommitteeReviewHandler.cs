using DoctoralManagement.Application.Common;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DoctoralManagement.Application.ThesisDefenseReviews
{
    public class SubmitCommitteeReviewHandler : IRequestHandler<SubmitCommitteeReviewCommand, CommitteeReviewResponse>
    {
        private readonly IThesisDefenseRepository _thesisDefenseRepository;
        private readonly ICommitteeReviewRepository _committeeReviewRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<SubmitCommitteeReviewHandler> _logger;

        public SubmitCommitteeReviewHandler(IThesisDefenseRepository thesisDefenseRepository, ICommitteeReviewRepository committeeReviewRepository, ICurrentUserService currentUserService, ILogger<SubmitCommitteeReviewHandler> logger)
        {
            _thesisDefenseRepository = thesisDefenseRepository;
            _committeeReviewRepository = committeeReviewRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<CommitteeReviewResponse> Handle(SubmitCommitteeReviewCommand request, CancellationToken cancellationToken)
        {
            var defense = await _thesisDefenseRepository.GetByIdAsync(request.DefenseId)
                ?? throw new DoctoralManagementException("Defense not found", HttpStatusCode.NotFound);

            var currentUserId = _currentUserService.UserId;
            var currentUserRole = _currentUserService.Role;

            if (currentUserRole != "Committee")
            {
                throw new DoctoralManagementException(
                    "Only committee members can submit reviews.",
                    HttpStatusCode.Forbidden);
            }

            if (!defense.CommitteeMemberIds.Contains(currentUserId))
            {
                throw new DoctoralManagementException(
                    "You are not part of the committee for this defense.",
                    HttpStatusCode.Forbidden);
            }

            if (defense.Status != Domain.Entities.DefenseStatus.Completed)
            {
                throw new DoctoralManagementException("Committee reviews can only be submitted AFTER the defense is completed.", HttpStatusCode.BadRequest);
            }

            if (!defense.CommitteeMemberIds.Contains(request.ReviewerId))
            {
                throw new DoctoralManagementException("Reviewer is not part of the committee", HttpStatusCode.BadRequest);
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

            _logger.LogInformation(
                "Committee member {ReviewerId} submitted review for defense {DefenseId}. Status: {ApprovalStatus}",
                currentUserId, defense.Id, request.ApprovalStatus);

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
