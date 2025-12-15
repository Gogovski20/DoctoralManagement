using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Courses.Queries
{
    public class GetAllCourseEnrollmentsHandler : IRequestHandler<GetAllCourseEnrollmentsQuery, IEnumerable<GetAllCourseEnrollmentsResponse>>
    {
        private readonly ICourseEnrollmentRepository _courseEnrollmentRepository;

        public GetAllCourseEnrollmentsHandler(ICourseEnrollmentRepository courseEnrollmentRepository)
        {
            _courseEnrollmentRepository = courseEnrollmentRepository;
        }

        public async Task<IEnumerable<GetAllCourseEnrollmentsResponse>> Handle(GetAllCourseEnrollmentsQuery request, CancellationToken cancellationToken)
        {
            var enrollments = await _courseEnrollmentRepository.GetCourseEnrollments();

            return enrollments.Select(c => new GetAllCourseEnrollmentsResponse
            {
                Id = c.Id,
                StudentId = c.StudentId,
                StudentName = c.Student.FullName,
                StudentIndex = c.Student.IndexNumber,
                CourseTitle = c.Course.Title,
                EnrolledDate = c.EnrolledDate,
                Completed = c.Completed,
                Grade = c.Grade ?? 0
            });
        }
    }
}
