using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.DoctoralProjects.Commands
{
    public class DeleteDoctoralProjectHandler : IRequestHandler<DeleteDoctoralProjectCommand, bool>
    {
        private readonly IDoctoralProjectRepository _repository;

        public DeleteDoctoralProjectHandler(IDoctoralProjectRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteDoctoralProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _repository.GetByIdAsync(request.Id)
                ?? throw new Exception($"Doctoral project with id {request.Id} not found.");

            if (project.Status != Domain.Entities.ProjectStatus.Draft)
            {
                throw new Exception("Only projects in 'Draft' status can be deleted.");
            }

            await _repository.DeleteAsync(request.Id);
            return true;
        }
    }
}
