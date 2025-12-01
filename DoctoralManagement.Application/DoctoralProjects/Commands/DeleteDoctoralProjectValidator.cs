using FluentValidation;

namespace DoctoralManagement.Application.DoctoralProjects.Commands
{
    public class DeleteDoctoralProjectValidator : AbstractValidator<DeleteDoctoralProjectCommand>
    {
        public DeleteDoctoralProjectValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);
        }
    }
}
