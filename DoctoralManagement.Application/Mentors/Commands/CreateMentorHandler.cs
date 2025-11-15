using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Mentors.Commands
{
    public class CreateMentorHandler : IRequestHandler<CreateMentorCommand, MentorResponse>
    {
        private readonly IMentorRepository _mentorRepository;

        public CreateMentorHandler(IMentorRepository mentorRepository)
        {
            _mentorRepository = mentorRepository;
        }

        public async Task<MentorResponse> Handle(CreateMentorCommand request, CancellationToken cancellationToken)
        {
            var mentor = new Mentor
            {
                FullName = request.FullName,
                Department = request.Department,
                Email = request.Email,
                Title = request.Title,
                MaxStudents = request.MaxStudents,
                IsActive = true,
                ResearchAreas = request.ResearchAreas
            };

            var createdMentor = await _mentorRepository.AddAsync(mentor);

            return new MentorResponse
            {
                Id = createdMentor.Id,
                FullName = createdMentor.FullName,
                Department = createdMentor.Department,
                Email = createdMentor.Email,
                Title = createdMentor.Title,
                MaxStudents = createdMentor.MaxStudents,
                IsActive = createdMentor.IsActive,
                ResearchAreas = createdMentor.ResearchAreas
            };
        }
    }
}
