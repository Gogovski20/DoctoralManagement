using FluentValidation;

namespace DoctoralManagement.Application.Mentors.Commands
{
    public class DeleteMentorValidator : AbstractValidator<DeleteMentorCommand>
    {
        public DeleteMentorValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);
        }
    }
}
