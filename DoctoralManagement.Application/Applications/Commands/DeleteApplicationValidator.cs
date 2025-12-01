using FluentValidation;

namespace DoctoralManagement.Application.Applications.Commands
{
    public class DeleteApplicationValidator : AbstractValidator<DeleteApplicationCommand>
    {
        public DeleteApplicationValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);
        }
    }
}
