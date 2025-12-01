using FluentValidation;

namespace DoctoralManagement.Application.Publications.Commands
{
    public class DeletePublicationValidator : AbstractValidator<DeletePublicationCommand>
    {
        public DeletePublicationValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);
        }
    }
}
