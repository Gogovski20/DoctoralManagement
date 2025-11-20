using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Courses.Queries
{
    public class GetStudentEnrollmentsHandler : IRequestHandler<GetStudentEnrollmentsQuery, IEnumerable<StudentEnrollmentResponse>>
    {
        private readonly ICourseEnrollmentRepository _courseEnrollmentRepository;

        public GetStudentEnrollmentsHandler(ICourseEnrollmentRepository courseEnrollmentRepository)
        {
            _courseEnrollmentRepository = courseEnrollmentRepository;
        }

        public async Task<IEnumerable<StudentEnrollmentResponse>> Handle(GetStudentEnrollmentsQuery request, CancellationToken cancellationToken)
        {
            var enrollments = await _courseEnrollmentRepository.GetByStudentIdAsync(request.StudentId);

            return enrollments.Select(e => new StudentEnrollmentResponse
            {
                EnrollmentId = e.Id,
                CourseId = e.CourseId,
                CourseCode = e.Course.Code,
                CourseName = e.Course.Title,
                EctsCredits = e.Course.EctsCredits,
                Completed = e.Completed,
                Grade = e.Grade,
                EnrolledDate = e.EnrolledDate
            }).ToList();
        }
    }
}
