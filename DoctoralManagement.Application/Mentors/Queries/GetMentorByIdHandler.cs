using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Mentors.Queries
{
    public class GetMentorByIdHandler : IRequestHandler<GetMentorByIdQuery, GetMentorResponse>
    {
        private readonly IMentorRepository _mentorRepository;

        public GetMentorByIdHandler(IMentorRepository mentorRepository)
        {
            _mentorRepository = mentorRepository;
        }

        public async Task<GetMentorResponse> Handle(GetMentorByIdQuery request, CancellationToken cancellationToken)
        {
            var mentor = await _mentorRepository.GetByIdAsync(request.Id);
            if (mentor == null)
            {
                throw new Exception("Mentor not found");
            }

            return new GetMentorResponse
            {
                Id = mentor.Id,
                FullName = mentor.FullName,
                Department = mentor.Department,
                Email = mentor.Email,
                Title = mentor.Title,
                MaxStudents = mentor.MaxStudents,
                IsActive = mentor.IsActive,
                ResearchAreas = mentor.ResearchAreas
            };
        }
    }
}
