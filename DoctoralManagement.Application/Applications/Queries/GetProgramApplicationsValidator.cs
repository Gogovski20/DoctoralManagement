using FluentValidation;

namespace DoctoralManagement.Application.Applications.Queries
{
    public class GetProgramApplicationsValidator : AbstractValidator<GetProgramApplicationsQuery>
    {
        public GetProgramApplicationsValidator()
        {
            RuleFor(x => x.ProgramId)
                .GreaterThan(0);

            RuleFor(x => x.Status)
                .IsInEnum()
                .When(x => x.Status.HasValue);
        }
    }
}
