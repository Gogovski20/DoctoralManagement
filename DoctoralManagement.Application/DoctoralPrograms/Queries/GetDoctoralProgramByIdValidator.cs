using FluentValidation;

namespace DoctoralManagement.Application.DoctoralPrograms.Queries
{
    public class GetDoctoralProgramByIdValidator : AbstractValidator<GetDoctoralProgramByIdQuery>
    {
        public GetDoctoralProgramByIdValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);
        }
    }
}
