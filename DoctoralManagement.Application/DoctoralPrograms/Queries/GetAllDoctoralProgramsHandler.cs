using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.DoctoralPrograms.Queries
{
    public class GetAllDoctoralProgramsHandler : IRequestHandler<GetAllDoctoralProgramsQuery, IEnumerable<GetAllDoctoralProgramsResponse>>
    {
        private readonly IDoctoralProgramRepository _repository;

        public GetAllDoctoralProgramsHandler(IDoctoralProgramRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<GetAllDoctoralProgramsResponse>> Handle(GetAllDoctoralProgramsQuery request, CancellationToken cancellationToken)
        {
            var programs = await _repository.GetAllAsync();

            return programs.Select(p => new GetAllDoctoralProgramsResponse
            {
                Id = p.Id,
                Name = p.Name,
                ScientificArea = p.ScientificArea,
                Faculty = p.Faculty,
                AvailableSlots = p.AvailableSlots,
                TuitionFee = p.TuitionFee,
                InternationalTuitionFee = p.InternationalTuitionFee,
                SpecialRequirements = p.SpecialRequirements,
                IsActive = p.IsActive,
                CurrentStudentsCount = p.Students?.Count ?? 0 // Calculate current students count
            });
        }
    }
}
