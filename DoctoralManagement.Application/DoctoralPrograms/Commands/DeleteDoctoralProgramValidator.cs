using FluentValidation;

namespace DoctoralManagement.Application.DoctoralPrograms.Commands
{
    public class DeleteDoctoralProgramValidator : AbstractValidator<DeleteDoctoralProgramCommand>
    {
        public DeleteDoctoralProgramValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);
        }
    }
}
