using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Mentors.Commands
{
    public class UpdateMentorHandler : IRequestHandler<UpdateMentorCommand, MentorResponse>
    {
        private readonly IMentorRepository _mentorRepository;

        public UpdateMentorHandler(IMentorRepository mentorRepository)
        {
            _mentorRepository = mentorRepository;
        }

        public async Task<MentorResponse> Handle(UpdateMentorCommand request, CancellationToken cancellationToken)
        {
            var mentor = await _mentorRepository.GetByIdAsync(request.Id)
                ?? throw new Exception($"Mentor with id {request.Id} not found");

            mentor.FullName = request.FullName;
            mentor.Department = request.Department;
            mentor.Title = request.Title;
            mentor.MaxStudents = request.MaxStudents;
            mentor.IsActive = request.IsActive;
            mentor.ResearchAreas = request.ResearchAreas;

            await _mentorRepository.UpdateAsync(mentor);

            return new MentorResponse
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
