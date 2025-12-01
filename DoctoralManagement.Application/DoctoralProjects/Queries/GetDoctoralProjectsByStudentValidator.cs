using FluentValidation;

namespace DoctoralManagement.Application.DoctoralProjects.Queries
{
    public class GetDoctoralProjectsByStudentValidator : AbstractValidator<GetDoctoralProjectsByStudentQuery>
    {
        public GetDoctoralProjectsByStudentValidator()
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0);
        }
    }
}
