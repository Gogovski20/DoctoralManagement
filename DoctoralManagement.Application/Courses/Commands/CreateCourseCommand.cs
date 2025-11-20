using MediatR;

namespace DoctoralManagement.Application.Courses.Commands
{
    public class CreateCourseCommand : IRequest<CreateCourseResponse>
    {
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int EctsCredits { get; set; }
        public string InstructorName { get; set; } = string.Empty;
        public int Semester { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class CreateCourseResponse
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
    }
}
