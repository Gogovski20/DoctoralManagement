using FluentValidation;

namespace DoctoralManagement.Application.DoctoralPrograms.Commands
{
    public class UpdateDoctoralProgramValidator : AbstractValidator<UpdateDoctoralProgramCommand>
    {
        public UpdateDoctoralProgramValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.ScientificArea)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Faculty)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.AvailableSlots)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.TuitionFee)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.InternationalTuitionFee)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.SpecialRequirements)
                .MaximumLength(2000);
        }
    }
}
