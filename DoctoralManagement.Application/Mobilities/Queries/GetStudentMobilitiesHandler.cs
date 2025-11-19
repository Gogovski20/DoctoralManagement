using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Mobilities.Queries
{
    public class GetStudentMobilitiesHandler : IRequestHandler<GetStudentMobilitiesQuery, IEnumerable<MobilityResponse>>
    {
        private readonly IMobilityRepository _mobilityRepository;

        public GetStudentMobilitiesHandler(IMobilityRepository mobilityRepository)
        {
            _mobilityRepository = mobilityRepository;
        }

        public async Task<IEnumerable<MobilityResponse>> Handle(GetStudentMobilitiesQuery request, CancellationToken cancellationToken)
        {
            var mobilities = await _mobilityRepository.GetByStudentIdAsync(request.StudentId);

            return mobilities.Select(m => new MobilityResponse
            {
                Id = m.Id,
                Institution = m.Institution,
                Country = m.Country,
                StartDate = m.StartDate,
                EndDate = m.EndDate
            }).ToList();
        }
    }
}
