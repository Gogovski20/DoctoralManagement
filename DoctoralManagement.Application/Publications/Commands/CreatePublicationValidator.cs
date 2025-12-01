using FluentValidation;

namespace DoctoralManagement.Application.Publications.Commands
{
    public class CreatePublicationValidator : AbstractValidator<CreatePublicationCommand>
    {
        public CreatePublicationValidator()
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0);

            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(300);

            RuleFor(x => x.Journal)
                .NotEmpty()
                .MaximumLength(300);

            RuleFor(x => x.PublishedOn)
                .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1)); 

            RuleFor(x => x.Doi)
                .MaximumLength(200);

            RuleFor(x => x.PossibleEctsCredits)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(30);
        }
    }
}
