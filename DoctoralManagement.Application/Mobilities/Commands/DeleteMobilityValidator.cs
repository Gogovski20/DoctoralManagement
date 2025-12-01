using FluentValidation;

namespace DoctoralManagement.Application.Mobilities.Commands
{
    public class DeleteMobilityValidator : AbstractValidator<DeleteMobilityCommand>
    {
        public DeleteMobilityValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);
        }
    }
}
