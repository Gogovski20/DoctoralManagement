using MediatR;

namespace DoctoralManagement.Application.Mentors.Commands
{
    public class CreateMentorCommand : IRequest<MentorResponse>
    {
        public string FullName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int MaxStudents { get; set; } = 5;
        public List<string> ResearchAreas { get; set; } = new();
    }
}
