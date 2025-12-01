using FluentValidation;

namespace DoctoralManagement.Application.Applications.Commands
{
    public class UpdateApplicationValidator : AbstractValidator<UpdateApplicationCommand>
    {
        public UpdateApplicationValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);

            RuleFor(x => x.PreferredMentorId)
                .GreaterThan(0)
                .When(x => x.PreferredMentorId.HasValue);
        }
    }
}
