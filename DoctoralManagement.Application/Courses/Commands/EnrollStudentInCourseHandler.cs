using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Courses.Commands
{
    public class EnrollStudentInCourseHandler : IRequestHandler<EnrollStudentInCourseCommand, EnrollStudentInCourseResponse>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ICourseEnrollmentRepository _courseEnrollmentRepository;
        private readonly IApplicationRepository _applicationRepository;

        public EnrollStudentInCourseHandler(IStudentRepository studentRepository, ICourseRepository courseRepository, ICourseEnrollmentRepository courseEnrollmentRepository, IApplicationRepository applicationRepository)
        {
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
            _courseEnrollmentRepository = courseEnrollmentRepository;
            _applicationRepository = applicationRepository;
        }

        public async Task<EnrollStudentInCourseResponse> Handle(EnrollStudentInCourseCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId)
                ?? throw new Exception($"Student with id {request.StudentId} not found");

            var hasFinalAccepted = await _applicationRepository.HasFinalAcceptedApplicationAsync(student.Id);
            if (!hasFinalAccepted) 
            {
                throw new Exception("Student must have final accepted application to enroll in courses");
            }

            var course = await _courseRepository.GetByIdAsync(request.CourseId)
                ?? throw new Exception($"Course with id {request.CourseId} not found");

            var existingEnrollment = await _courseEnrollmentRepository.GetStudentCourseEnrollmentAsync(request.StudentId, request.CourseId);
            if (existingEnrollment != null)
            {
                throw new Exception("Student is already enrolled in this course");
            }

            var enrollment = new CourseEnrollment
            {
                StudentId = request.StudentId,
                CourseId = request.CourseId,
                EnrolledDate = DateTime.UtcNow,
                Completed = false
            };

            var created = await _courseEnrollmentRepository.AddAsync(enrollment);

            return new EnrollStudentInCourseResponse
            {
                EnrollmentId = created.Id,
                StudentId = created.StudentId,
                CourseName = course.Title,
                EnrolledDate = created.EnrolledDate
            };
        }
    }
}
