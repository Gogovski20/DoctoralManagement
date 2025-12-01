using FluentValidation;

namespace DoctoralManagement.Application.Courses.Queries
{
    public class GetCourseByIdValidator : AbstractValidator<GetCourseByIdQuery>
    {
        public GetCourseByIdValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);
        }
    }
}
