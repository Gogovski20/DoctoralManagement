using FluentValidation;

namespace DoctoralManagement.Application.ConferenceParticipations.Commands
{
    public class ReviewConferenceValidator : AbstractValidator<ReviewConferenceCommand>
    {
        public ReviewConferenceValidator()
        {
            RuleFor(x => x.ConferenceId)
                .GreaterThan(0);

            RuleFor(x => x.ReviewComments)
                .MaximumLength(1000);
        }
    }
}
