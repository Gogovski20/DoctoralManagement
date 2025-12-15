using MediatR;

namespace DoctoralManagement.Application.Courses.Queries
{
    public class GetAllCourseEnrollmentsQuery : IRequest<IEnumerable<GetAllCourseEnrollmentsResponse>>
    {
    }

    public class GetAllCourseEnrollmentsResponse
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentIndex { get; set; } = string.Empty;
        public string CourseTitle { get; set; } = string.Empty;
        public DateTime EnrolledDate { get; set; }
        public bool Completed { get; set; } = false;
        public decimal? Grade { get; set; } 
    }
}
