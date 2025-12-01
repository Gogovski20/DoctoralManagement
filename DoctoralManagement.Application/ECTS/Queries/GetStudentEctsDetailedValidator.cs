using FluentValidation;

namespace DoctoralManagement.Application.ECTS.Queries
{
    public class GetStudentEctsDetailedValidator : AbstractValidator<GetStudentEctsDetailedQuery>
    {
        public GetStudentEctsDetailedValidator()
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0);
        }
    }
}
