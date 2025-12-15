using FluentValidation;

namespace DoctoralManagement.Application.Publications.Commands
{
    public class UpdatePublicationValidator : AbstractValidator<UpdatePublicationCommand>
    {
        public UpdatePublicationValidator()
        {
            RuleFor(x => x.Id)
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
        }
    }
}
