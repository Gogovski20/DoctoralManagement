using DoctoralManagement.Application.Common;
using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DoctoralManagement.Application.Courses.Commands
{
    public class EnrollStudentInCourseHandler : IRequestHandler<EnrollStudentInCourseCommand, EnrollStudentInCourseResponse>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ICourseEnrollmentRepository _courseEnrollmentRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;
        private readonly ILogger<EnrollStudentInCourseHandler> _logger;

        public EnrollStudentInCourseHandler(IStudentRepository studentRepository, ICourseRepository courseRepository, ICourseEnrollmentRepository courseEnrollmentRepository, IApplicationRepository applicationRepository, ICurrentUserService currentUserService, IAuthService authService, ILogger<EnrollStudentInCourseHandler> logger)
        {
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
            _courseEnrollmentRepository = courseEnrollmentRepository;
            _applicationRepository = applicationRepository;
            _currentUserService = currentUserService;
            _authService = authService;
            _logger = logger;
        }

        public async Task<EnrollStudentInCourseResponse> Handle(EnrollStudentInCourseCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            var currentUserRole = _currentUserService.Role;
            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);
            var isAdmin = currentUserRole == "Admin";

            if (!isAdmin && (linkedStudentId == null || linkedStudentId != request.StudentId))
            {
                throw new DoctoralManagementException(
                    "You can only enroll yourself in courses.",
                    HttpStatusCode.Forbidden);
            }

            var student = await _studentRepository.GetByIdAsync(request.StudentId)
                ?? throw new DoctoralManagementException($"Student with id {request.StudentId} not found", HttpStatusCode.NotFound);

            var hasFinalAccepted = await _applicationRepository.HasFinalAcceptedApplicationAsync(student.Id);
            if (!hasFinalAccepted) 
            {
                throw new DoctoralManagementException("Student must have final accepted application to enroll in courses", HttpStatusCode.BadRequest);
            }

            var course = await _courseRepository.GetByIdAsync(request.CourseId)
                ?? throw new DoctoralManagementException($"Course with id {request.CourseId} not found", HttpStatusCode.NotFound);

            var existingEnrollment = await _courseEnrollmentRepository.GetStudentCourseEnrollmentAsync(request.StudentId, request.CourseId);
            if (existingEnrollment != null)
            {
                throw new DoctoralManagementException("Student is already enrolled in this course", HttpStatusCode.BadRequest);
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
