using FluentValidation;

namespace DoctoralManagement.Application.Students.Queries
{
    public class GetStudentByIdValidator : AbstractValidator<GetStudentByIdQuery>
    {
        public GetStudentByIdValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);
        }
    }
}
