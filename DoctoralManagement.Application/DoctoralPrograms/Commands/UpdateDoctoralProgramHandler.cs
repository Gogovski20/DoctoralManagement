using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.DoctoralPrograms.Commands
{
    public class UpdateDoctoralProgramHandler : IRequestHandler<UpdateDoctoralProgramCommand, UpdateDoctoralProgramResponse>
    {
        private readonly IDoctoralProgramRepository _repository;

        public UpdateDoctoralProgramHandler(IDoctoralProgramRepository repository)
        {
            _repository = repository;
        }

        public async Task<UpdateDoctoralProgramResponse> Handle(UpdateDoctoralProgramCommand request, CancellationToken cancellationToken)
        {
            var doctoralProgram = await _repository.GetByIdAsync(request.Id);

            if (doctoralProgram == null)
            {
                throw new Exception($"Doctoral Program with ID {request.Id} not found.");
            }

            if (doctoralProgram.Name != request.Name && await _repository.ExistsByNameAsync(request.Name))
            {
                throw new Exception($"Another Doctoral Program with the name '{request.Name}' already exists.");
            }

            doctoralProgram.Name = request.Name;
            doctoralProgram.ScientificArea = request.ScientificArea;
            doctoralProgram.Faculty = request.Faculty;
            doctoralProgram.AvailableSlots = request.AvailableSlots;
            doctoralProgram.TuitionFee = request.TuitionFee;
            doctoralProgram.InternationalTuitionFee = request.InternationalTuitionFee;
            doctoralProgram.SpecialRequirements = request.SpecialRequirements;
            doctoralProgram.IsActive = request.IsActive;

            await _repository.UpdateAsync(doctoralProgram);

            return new UpdateDoctoralProgramResponse
            {
                Id = doctoralProgram.Id,
                Name = doctoralProgram.Name,
                ScientificArea = doctoralProgram.ScientificArea,
                Faculty = doctoralProgram.Faculty,
                AvailableSlots = doctoralProgram.AvailableSlots,
                TuitionFee = doctoralProgram.TuitionFee,
                InternationalTuitionFee = doctoralProgram.InternationalTuitionFee,
                SpecialRequirements = doctoralProgram.SpecialRequirements,
                IsActive = doctoralProgram.IsActive
            };
        }
    }
}
