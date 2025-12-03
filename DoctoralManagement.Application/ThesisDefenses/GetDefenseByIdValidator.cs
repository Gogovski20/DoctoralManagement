using FluentValidation;

namespace DoctoralManagement.Application.ThesisDefenses
{
    public class GetDefenseByIdValidator : AbstractValidator<GetDefenseByIdQuery>
    {
        public GetDefenseByIdValidator()
        {
            RuleFor(x => x.DefenseId)
                .GreaterThan(0);
        }
    }
}
