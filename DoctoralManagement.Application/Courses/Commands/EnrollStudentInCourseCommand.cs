using MediatR;

namespace DoctoralManagement.Application.Courses.Commands
{
    public class EnrollStudentInCourseCommand : IRequest<EnrollStudentInCourseResponse>
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
    }

    public class EnrollStudentInCourseResponse
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public DateTime EnrolledDate { get; set; }
    }
}
