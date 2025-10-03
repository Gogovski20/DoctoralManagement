using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.DoctoralPrograms.Queries
{
    public class GetDoctoralProgramByIdHandler : IRequestHandler<GetDoctoralProgramByIdQuery, GetDoctoralProgramByIdResponse>
    {
        private readonly IDoctoralProgramRepository _repository;

        public GetDoctoralProgramByIdHandler(IDoctoralProgramRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetDoctoralProgramByIdResponse> Handle(GetDoctoralProgramByIdQuery request, CancellationToken cancellationToken)
        {
            var doctoralProgram = await _repository.GetByIdAsync(request.Id);

            if (doctoralProgram == null)
            {
                throw new Exception($"Doctoral Program with ID {request.Id} not found.");
            }

            return new GetDoctoralProgramByIdResponse
            {
                Id = doctoralProgram.Id,
                Name = doctoralProgram.Name,
                ScientificArea = doctoralProgram.ScientificArea,
                Faculty = doctoralProgram.Faculty,
                AvailableSlots = doctoralProgram.AvailableSlots,
                TuitionFee = doctoralProgram.TuitionFee,
                InternationalTuitionFee = doctoralProgram.InternationalTuitionFee,
                SpecialRequirements = doctoralProgram.SpecialRequirements,
                IsActive = doctoralProgram.IsActive,
                CurrentStudentsCount = doctoralProgram.Students?.Count ?? 0
            };
        }
    }
}
