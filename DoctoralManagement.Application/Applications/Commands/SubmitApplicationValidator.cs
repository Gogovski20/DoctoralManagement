using FluentValidation;

namespace DoctoralManagement.Application.Applications.Commands
{
    public class SubmitApplicationValidator : AbstractValidator<SubmitApplicationCommand>
    {
        public SubmitApplicationValidator()
        {
            RuleFor(x => x.ApplicationId)
                .GreaterThan(0);
        }
    }
}
