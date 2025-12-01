using FluentValidation;

namespace DoctoralManagement.Application.Applications.Queries
{
    public class GetAllApplicationValidator : AbstractValidator<GetAllApplicationsQuery>
    {
        public GetAllApplicationValidator()
        {
            RuleFor(x => x.ProgramId)
                .GreaterThan(0)
                .When(x => x.ProgramId.HasValue);

            RuleFor(x => x.StudentId)
                .GreaterThan(0)
                .When(x => x.StudentId.HasValue);

            RuleFor(x => x.Status)
                .IsInEnum()
                .When(x => x.Status.HasValue);
        }
    }
}
