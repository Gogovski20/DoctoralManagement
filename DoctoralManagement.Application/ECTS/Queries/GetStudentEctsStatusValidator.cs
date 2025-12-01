using FluentValidation;

namespace DoctoralManagement.Application.ECTS.Queries
{
    public class GetStudentEctsStatusValidator : AbstractValidator<GetStudentEctsStatusQuery>
    {
        public GetStudentEctsStatusValidator()
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0);
        }
    }
}
