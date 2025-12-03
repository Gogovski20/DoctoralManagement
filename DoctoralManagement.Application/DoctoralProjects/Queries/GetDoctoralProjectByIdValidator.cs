using FluentValidation;

namespace DoctoralManagement.Application.DoctoralProjects.Queries
{
    internal class GetDoctoralProjectByIdValidator : AbstractValidator<GetDoctoralProjectByIdQuery>
    {
        public GetDoctoralProjectByIdValidator()
        {
            RuleFor(x => x.DoctoralProjectId)
               .GreaterThan(0);
        }
    }
}
