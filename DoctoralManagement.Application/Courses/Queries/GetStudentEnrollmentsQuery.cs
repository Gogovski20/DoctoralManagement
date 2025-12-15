using MediatR;

namespace DoctoralManagement.Application.Courses.Queries
{
    public class GetStudentEnrollmentsQuery : IRequest<IEnumerable<StudentEnrollmentResponse>>
    {
        public int StudentId { get; set; }
    }

    public class StudentEnrollmentResponse
    {
        public int EnrollmentId { get; set; }
        public int CourseId { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public int EctsCredits { get; set; }
        public bool Completed { get; set; }
        public decimal? Grade { get; set; }
        public DateTime EnrolledDate { get; set; }
    }
}
