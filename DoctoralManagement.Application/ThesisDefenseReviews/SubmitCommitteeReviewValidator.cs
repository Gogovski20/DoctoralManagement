using FluentValidation;

namespace DoctoralManagement.Application.ThesisDefenseReviews
{
    public class SubmitCommitteeReviewValidator : AbstractValidator<SubmitCommitteeReviewCommand>
    {
        public SubmitCommitteeReviewValidator()
        {
            RuleFor(x => x.DefenseId)
                .GreaterThan(0);

            RuleFor(x => x.ReviewerId)
                .GreaterThan(0);

            RuleFor(x => x.Comments)
                .MaximumLength(2000);

            RuleFor(x => x.ApprovalStatus)
                .IsInEnum();
        }
    }
}
