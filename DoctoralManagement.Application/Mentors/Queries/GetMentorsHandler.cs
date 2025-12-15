using DoctoralManagement.Application.Dtos;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Mentors.Queries
{
    public class GetMentorsHandler : IRequestHandler<GetMentorsQuery, List<MentorLookupDto>>
    {
        private readonly IMentorRepository _repository;

        public GetMentorsHandler(IMentorRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<MentorLookupDto>> Handle(GetMentorsQuery request, CancellationToken cancellationToken)
        {
            var mentors = await _repository.GetAllAsync();

            return mentors
                .Select(m => new MentorLookupDto
                {
                    Id = m.Id,
                    FullName = m.FullName
                }).ToList();
        }
    }
}
