using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.DoctoralPrograms.Commands
{
    public class DeleteDoctoralProgramHandler : IRequestHandler<DeleteDoctoralProgramCommand, DeleteDoctoralProgramResponse>
    {
        private readonly IDoctoralProgramRepository _repository;

        public DeleteDoctoralProgramHandler(IDoctoralProgramRepository repository)
        {
            _repository = repository;
        }

        public async Task<DeleteDoctoralProgramResponse> Handle(DeleteDoctoralProgramCommand request, CancellationToken cancellationToken)
        {
            var doctoralProgram = await _repository.GetByIdAsync(request.Id);

            if (doctoralProgram == null)
            {
                throw new Exception($"Doctoral program with ID {request.Id} not found.");
            }

            await _repository.DeleteAsync(doctoralProgram);

            return new DeleteDoctoralProgramResponse();
        }
    }
}
