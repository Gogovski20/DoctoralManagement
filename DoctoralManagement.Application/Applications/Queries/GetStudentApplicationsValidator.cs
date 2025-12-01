using FluentValidation;

namespace DoctoralManagement.Application.Applications.Queries
{
    public class GetStudentApplicationsValidator : AbstractValidator<GetStudentApplicationsQuery>
    {
        public GetStudentApplicationsValidator()
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0);
        }
    }
}
