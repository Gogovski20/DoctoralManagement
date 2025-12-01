using FluentValidation;

namespace DoctoralManagement.Application.DoctoralProjects.Queries
{
    public class GetDoctoralProjectsByMentorValidator : AbstractValidator<GetDoctoralProjectsByMentorQuery>
    {
        public GetDoctoralProjectsByMentorValidator()
        {
            RuleFor(x => x.MentorId)
                .GreaterThan(0);
        }
    }
}
