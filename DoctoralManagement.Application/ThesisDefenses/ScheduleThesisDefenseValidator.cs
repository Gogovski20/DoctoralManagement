using FluentValidation;

namespace DoctoralManagement.Application.ThesisDefenses
{
    public class ScheduleThesisDefenseValidator : AbstractValidator<ScheduleThesisDefenseCommand>
    {
        public ScheduleThesisDefenseValidator()
        {
            RuleFor(x => x.ProjectId)
                .GreaterThan(0);

            RuleFor(x => x.ScheduledAt)
                .GreaterThan(DateTime.UtcNow.AddMinutes(-5)); 

            RuleFor(x => x.Room)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.CommitteeMemberIds)
                .NotNull()
                .Must(list => list.Count >= 3)
                    .WithMessage("At least 3 committee members are required.");
        }
    }
}
