using DoctoralManagement.Application.Dtos;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Mobilities.Queries
{
    public class GetAllMobilitiesHandler : IRequestHandler<GetAllMobilitiesQuery, IEnumerable<GetAllMobilitiesResponse>>
    {
        private readonly IMobilityRepository _mobilityRepository;

        public GetAllMobilitiesHandler(IMobilityRepository mobilityRepository)
        {
            _mobilityRepository = mobilityRepository;
        }

        public async Task<IEnumerable<GetAllMobilitiesResponse>> Handle(GetAllMobilitiesQuery request, CancellationToken cancellationToken)
        {
            var mobilities = await _mobilityRepository.GetAllAsync();

            return mobilities.Select(m => new GetAllMobilitiesResponse
            {
                Id = m.Id,
                StudentName = m.Student?.FullName ?? "N/A",
                Institution = m.Institution,
                Country = m.Country,
                StartDate = m.StartDate,
                EndDate = m.EndDate,
                Document = m.Document == null ? null : new DocumentDto
                {
                    Id = m.Document.Id,
                    FileName = m.Document.FileName
                }
            });
        }
    }
}
