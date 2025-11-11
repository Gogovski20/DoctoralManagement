using MediatR;

namespace DoctoralManagement.Application.Mentors.Commands
{
    public class UpdateMentorCommand : IRequest<MentorResponse>
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int MaxStudents { get; set; } = 5;
        public bool IsActive { get; set; }
        public List<string> ResearchAreas { get; set; } = new();
    }
}
