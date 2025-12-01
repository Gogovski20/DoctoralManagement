using FluentValidation;

namespace DoctoralManagement.Application.ConferenceParticipations.Queries
{
    public class GetStudentConferencesValidator : AbstractValidator<GetStudentConferencesQuery>
    {
        public GetStudentConferencesValidator()
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0);
        }
    }
}
