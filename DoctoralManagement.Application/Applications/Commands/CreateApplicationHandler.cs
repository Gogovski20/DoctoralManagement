using DoctoralManagement.Application.Common;
using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Applications.Commands
{
    public class CreateApplicationHandler : IRequestHandler<CreateApplicationCommand, CreateApplicationResponse>
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IDoctoralProgramRepository _doctoralProgramRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;

        public CreateApplicationHandler(IApplicationRepository applicationRepository, IStudentRepository studentRepository, IDoctoralProgramRepository doctoralProgramRepository, ICurrentUserService currentUserService, IAuthService authService)
        {
            _applicationRepository = applicationRepository;
            _studentRepository = studentRepository;
            _doctoralProgramRepository = doctoralProgramRepository;
            _currentUserService = currentUserService;
            _authService = authService;
        }

        public async Task<CreateApplicationResponse> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId);

            if (student == null)
            {
                throw new Exception($"Student with ID: {request.StudentId} not found");
            }

            var currentUserId = _currentUserService.UserId;

            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);

            if (linkedStudentId == null)
            {
                throw new UnauthorizedAccessException("Current user is not linked to a student record");
            }

            if (request.StudentId != linkedStudentId)
            {
                throw new UnauthorizedAccessException("You can only submit your own application");
            }

            var program = await _doctoralProgramRepository.GetByIdAsync(request.DoctoralProgramId);

            if (program == null)
            {
                throw new Exception($"Doctoral Program with ID: {request.DoctoralProgramId} not found");
            }

            if (program.CurrentStudentsCount >= program.AvailableSlots)
            {
                throw new Exception($"Program '{program.Name}' has no available slots. Current: {program.CurrentStudentsCount}/{program.AvailableSlots}");
            }

            var meetsGradeRequirements = student.GPA >= 8.00m;
            var hasRequiredCredits = student.TotalCredits >= 300;

            if (!meetsGradeRequirements)
            {
                throw new Exception("GPA must be at least 8.00 for doctoral studies!");
            }

            if (!hasRequiredCredits)
            {
                throw new Exception("Student must have at least 300 ECTS credits from previous studies!");
            }

            if (await _applicationRepository.HasActiveApplicationAsync(request.StudentId, request.DoctoralProgramId))
            {
                throw new Exception("Student already has an active application for this program");
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
