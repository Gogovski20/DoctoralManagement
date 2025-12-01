using FluentValidation;

namespace DoctoralManagement.Application.ThesisDefenses
{
    public class CompleteThesisDefenseValidator : AbstractValidator<CompleteThesisDefenseCommand>
    {
        public CompleteThesisDefenseValidator()
        {
            RuleFor(x => x.DefenseId)
                .GreaterThan(0);

            RuleFor(x => x.ResultNotes)
                .MaximumLength(2000);

            RuleFor(x => x.ArchiveNumber)
                .MaximumLength(100);
        }
    }
}
