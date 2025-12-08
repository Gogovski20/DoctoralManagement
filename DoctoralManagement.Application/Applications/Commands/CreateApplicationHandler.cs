using DoctoralManagement.Application.Common;
using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DoctoralManagement.Application.Applications.Commands
{
    public class CreateApplicationHandler : IRequestHandler<CreateApplicationCommand, CreateApplicationResponse>
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IDoctoralProgramRepository _doctoralProgramRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;
        private readonly ILogger<CreateApplicationHandler> _logger;

        public CreateApplicationHandler(IApplicationRepository applicationRepository, IStudentRepository studentRepository, IDoctoralProgramRepository doctoralProgramRepository, ICurrentUserService currentUserService, IAuthService authService, ILogger<CreateApplicationHandler> logger)
        {
            _applicationRepository = applicationRepository;
            _studentRepository = studentRepository;
            _doctoralProgramRepository = doctoralProgramRepository;
            _currentUserService = currentUserService;
            _authService = authService;
            _logger = logger;
        }

        public async Task<CreateApplicationResponse> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId);

            if (student == null)
            {
                throw new DoctoralManagementException($"Student with ID: {request.StudentId} not found", HttpStatusCode.NotFound);
            }

            var currentUserId = _currentUserService.UserId;
            _logger.LogInformation($"Current User ID from service: {currentUserId}");

            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);
            _logger.LogInformation($"Linked Student ID for User {currentUserId}: {linkedStudentId}");

            if (linkedStudentId == null)
            {
                _logger.LogWarning($"No student link found for user {currentUserId}");
                throw new DoctoralManagementException("Current user is not linked to a student record", HttpStatusCode.Forbidden);
            }

            if (request.StudentId != linkedStudentId)
            {
                throw new DoctoralManagementException("You can only submit your own application", HttpStatusCode.Forbidden);
            }

            var program = await _doctoralProgramRepository.GetByIdAsync(request.DoctoralProgramId);

            if (program == null)
            {
                throw new DoctoralManagementException($"Doctoral Program with ID: {request.DoctoralProgramId} not found", HttpStatusCode.NotFound);
            }

            if (program.CurrentStudentsCount >= program.AvailableSlots)
            {
                throw new DoctoralManagementException($"Program '{program.Name}' has no available slots. Current: {program.CurrentStudentsCount}/{program.AvailableSlots}", HttpStatusCode.BadRequest);
            }

            var meetsGradeRequirements = student.GPA >= 8.00m;
            var hasRequiredCredits = student.TotalCredits >= 300;

            if (!meetsGradeRequirements)
            {
                throw new DoctoralManagementException("GPA must be at least 8.00 for doctoral studies!", HttpStatusCode.BadRequest);
            }

            if (!hasRequiredCredits)
            {
                throw new DoctoralManagementException("Student must have at least 300 ECTS credits from previous studies!", HttpStatusCode.BadRequest);
            }

            if (await _applicationRepository.HasActiveApplicationAsync(request.StudentId, request.DoctoralProgramId))
            {
                throw new DoctoralManagementException("Student already has an active application for this program", HttpStatusCode.BadRequest);
            }

            var application = new Domain.Entities.Application
            {
                StudentId = request.StudentId,
                DoctoralProgramId = request.DoctoralProgramId,
                PrefferedMentorId = request.PreferredMentorId,
                ApplicationStatus = ApplicationStatus.Draft,
                ApplicationDate = DateTime.UtcNow,
                MeetsGradeRequirements = meetsGradeRequirements,
                HasRequiredPublications = false 
            };

            var createdApplication = await _applicationRepository.AddAsync(application);

            _logger.LogInformation("Application created");

            return new CreateApplicationResponse
            {
                Id = createdApplication.Id,
                StudentId = createdApplication.StudentId,
                DoctoralProgramId = createdApplication.DoctoralProgramId,
                PreferredMentorId = createdApplication.PrefferedMentorId,
                ApplicationStatus = createdApplication.ApplicationStatus,
                ApplicationDate = createdApplication.ApplicationDate,
                MeetsGradeRequirements = createdApplication.MeetsGradeRequirements,
                HasRequiredPublications = createdApplication.HasRequiredPublications
            };
        }
    }
}
