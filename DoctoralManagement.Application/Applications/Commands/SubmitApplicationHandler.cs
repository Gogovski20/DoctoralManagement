using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Applications.Commands
{
    public class SubmitApplicationHandler : IRequestHandler<SubmitApplicationCommand, SubmitApplicationResponse>
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IDoctoralProgramRepository _doctoralProgramRepository;

        public SubmitApplicationHandler(IApplicationRepository applicationRepository, IStudentRepository studentRepository, IDoctoralProgramRepository doctoralProgramRepository)
        {
            _applicationRepository = applicationRepository;
            _studentRepository = studentRepository;
            _doctoralProgramRepository = doctoralProgramRepository;
        }

        public async Task<SubmitApplicationResponse> Handle(SubmitApplicationCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId);

            if (student == null)
            {
                throw new Exception($"Student with ID: {request.StudentId} not found");
            }

            var program = await _doctoralProgramRepository.GetByIdAsync(request.DoctoralProgramId);

            if (program == null)
            {
                throw new Exception($"Doctoral Program with ID: {request.DoctoralProgramId} not found");
            }

            if (await _applicationRepository.HasActiveApplicationAsync(request.StudentId, request.DoctoralProgramId))
            {
                throw new Exception("Student already has an active application for this program");
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

            if (string.IsNullOrEmpty(request.EnglishCertificatePath))
            {
                throw new Exception("English certificate is required for application!");
            }

            var application = new Domain.Entities.Application
            {
                StudentId = request.StudentId,
                DoctoralProgramId = request.DoctoralProgramId,
                PrefferedMentorId = request.PreferredMentorId,
                MotivationLetter = request.MotivationLetter,
                ResearchProposal = request.ResearchProposal,
                EnglishCertificatePath = request.EnglishCertificatePath,
                ApplicationStatus = ApplicationStatus.Submitted,
                ApplicationDate = DateTime.UtcNow,
                MeetsGradeRequirements = meetsGradeRequirements,
                HasRequiredPublications = false // Initial assumption; can be updated later
            };

            var createdApplication = await _applicationRepository.AddAsync(application);

            return new SubmitApplicationResponse
            {
                Id = createdApplication.Id,
                StudentId = createdApplication.StudentId,
                DoctoralProgramId = createdApplication.DoctoralProgramId,
                PreferredMentorId = createdApplication.PrefferedMentorId,
                MotivationLetter = createdApplication.MotivationLetter,
                ResearchProposal = createdApplication.ResearchProposal,
                EnglishCertificatePath = createdApplication.EnglishCertificatePath,
                ApplicationStatus = createdApplication.ApplicationStatus,
                ApplicationDate = createdApplication.ApplicationDate,
                MeetsGradeRequirements = createdApplication.MeetsGradeRequirements,
                HasRequiredPublications = createdApplication.HasRequiredPublications
            };
        }
    }
}
