using FluentValidation;

namespace DoctoralManagement.Application.Publications.Queries
{
    public class GetStudentPublicationsValidator : AbstractValidator<GetStudentPublicationsQuery>
    {
        public GetStudentPublicationsValidator()
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0);
        }
    }
}
