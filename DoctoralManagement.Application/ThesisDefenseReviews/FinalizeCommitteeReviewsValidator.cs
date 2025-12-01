using FluentValidation;

namespace DoctoralManagement.Application.ThesisDefenseReviews
{
    public class FinalizeCommitteeReviewsValidator : AbstractValidator<FinalizeCommitteeReviewsCommand>
    {
        public FinalizeCommitteeReviewsValidator()
        {
            RuleFor(x => x.DefenseId)
                .GreaterThan(0);
        }
    }
}
