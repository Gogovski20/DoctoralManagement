using DoctoralManagement.Application.ECTS.Services;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using System.Net;

namespace DoctoralManagement.Application.Courses.Commands
{
    public class CompleteCourseEnrollmentHandler : IRequestHandler<CompleteCourseEnrollmentCommand, bool>
    {
        private readonly ICourseEnrollmentRepository _courseEnrollmentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IEctsTrackingRepository _ectsTrackingRepository;
        private readonly EctsProgressService _progressService;

        public CompleteCourseEnrollmentHandler(ICourseEnrollmentRepository courseEnrollmentRepository, ICourseRepository courseRepository, IEctsTrackingRepository ectsTrackingRepository, EctsProgressService progressService)
        {
            _courseEnrollmentRepository = courseEnrollmentRepository;
            _courseRepository = courseRepository;
            _ectsTrackingRepository = ectsTrackingRepository;
            _progressService = progressService;
        }

        public async Task<bool> Handle(CompleteCourseEnrollmentCommand request, CancellationToken cancellationToken)
        {
            var enrollment = await _courseEnrollmentRepository.GetByIdAsync(request.EnrollmentId)
                ?? throw new DoctoralManagementException($"Enrollment with id {request.EnrollmentId} not found", HttpStatusCode.NotFound);

            if (enrollment.StudentId != request.StudentId) 
            {
                throw new DoctoralManagementException("Student ID mismatch", HttpStatusCode.BadRequest);
            }

            var course = await _courseRepository.GetByIdAsync(enrollment.CourseId)
                ?? throw new DoctoralManagementException("Course not found", HttpStatusCode.NotFound);

            if (request.Grade < 5.0m || request.Grade > 10.0m)
            {
                throw new DoctoralManagementException("Grade must be between 5.0 and 10.0", HttpStatusCode.BadRequest);
            }

            enrollment.Completed = true;
            enrollment.Grade = request.Grade;
            await _courseEnrollmentRepository.UpdateAsync(enrollment);

            if (request.Grade > 5.0m)
            {
                var ectsTracking = await _ectsTrackingRepository.GetByStudentIdAsync(request.StudentId);
                if (ectsTracking != null)
                {
                    ectsTracking.OrganizedAcademicTraining += course.EctsCredits;
                    if (ectsTracking.OrganizedAcademicTraining > 42)
                    {
                        ectsTracking.OrganizedAcademicTraining = 42;
                    }
                    await _ectsTrackingRepository.UpdateAsync(ectsTracking);

                    await _progressService.UpdateStudentSemesterAsync(request.StudentId, ectsTracking.TotalECTS);
                }
            }
            return true;
        }
    }
}
