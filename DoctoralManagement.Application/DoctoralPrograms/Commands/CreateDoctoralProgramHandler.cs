using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.DoctoralPrograms.Commands
{
    public class CreateDoctoralProgramHandler : IRequestHandler<CreateDoctoralProgramCommand, CreateDoctoralProgramResponse>
    {
        private readonly IDoctoralProgramRepository _repository;

        public CreateDoctoralProgramHandler(IDoctoralProgramRepository repository)
        {
            _repository = repository;
        }

        public async Task<CreateDoctoralProgramResponse> Handle(CreateDoctoralProgramCommand request, CancellationToken cancellationToken)
        {
            if (await _repository.ExistsByNameAsync(request.Name))
            {
                throw new Exception($"A doctoral program with the name {request.Name} already exists.");
            }

            var doctoralProgram = new DoctoralProgram
            {
                Name = request.Name,
                ScientificArea = request.ScientificArea,
                Faculty = request.Faculty,
                AvailableSlots = request.AvailableSlots,
                TuitionFee = request.TuitionFee,
                InternationalTuitionFee = request.InternationalTuitionFee,
                SpecialRequirements = request.SpecialRequirements,
                IsActive = true
            };

            var createdProgram = await _repository.AddAsync(doctoralProgram);

            return new CreateDoctoralProgramResponse
            {
                Id = createdProgram.Id,
                Name = createdProgram.Name,
                ScientificArea = createdProgram.ScientificArea,
                Faculty = createdProgram.Faculty,
                AvailableSlots = createdProgram.AvailableSlots,
                TuitionFee = createdProgram.TuitionFee,
                InternationalTuitionFee = createdProgram.InternationalTuitionFee,
                SpecialRequirements = createdProgram.SpecialRequirements,
                IsActive = createdProgram.IsActive
            };
        }
    }
}
