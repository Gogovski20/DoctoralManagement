using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Mentors.Queries
{
    public class GetAllMentorsHandler : IRequestHandler<GetAllMentorsQuery, IEnumerable<GetMentorResponse>>
    {
        private readonly IMentorRepository _mentorRepository;

        public GetAllMentorsHandler(IMentorRepository mentorRepository)
        {
            _mentorRepository = mentorRepository;
        }

        public async Task<IEnumerable<GetMentorResponse>> Handle(GetAllMentorsQuery request, CancellationToken cancellationToken)
        {
            var mentors = await _mentorRepository.GetAllAsync();

            return mentors.Select(mentor => new GetMentorResponse
            {
                Id = mentor.Id,
                FullName = mentor.FullName,
                Department = mentor.Department,
                Email = mentor.Email,
                Title = mentor.Title,
                MaxStudents = mentor.MaxStudents,
                IsActive = mentor.IsActive,
                ResearchAreas = mentor.ResearchAreas
            });
        }
    }
}
