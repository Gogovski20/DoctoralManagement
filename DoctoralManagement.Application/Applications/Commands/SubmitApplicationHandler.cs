using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Applications.Commands
{
    public class SubmitApplicationHandler : IRequestHandler<SubmitApplicationCommand, SubmitApplicationResponse>
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IApplicationDocumentRepository _applicationDocumentRepository;

        public SubmitApplicationHandler(IApplicationRepository applicationRepository, IStudentRepository studentRepository, IApplicationDocumentRepository applicationDocumentRepository)
        {
            _applicationRepository = applicationRepository;
            _studentRepository = studentRepository;
            _applicationDocumentRepository = applicationDocumentRepository;
        }

        public async Task<SubmitApplicationResponse> Handle(SubmitApplicationCommand request, CancellationToken cancellationToken)
        {
            var application = await _applicationRepository.GetByIdAsync(request.ApplicationId)
                ?? throw new Exception("Application not found.");

            if (application.ApplicationStatus != Domain.Entities.ApplicationStatus.Draft)
            {
                throw new Exception("Only draft applications can be submitted.");
            }

            var hasRequiredDocuments = await _applicationDocumentRepository.HasAllRequiredDocumentsAsync(application.Id);
            if (!hasRequiredDocuments)
            {
                throw new Exception("Cannot submit application. Required documents are missing.");
            }

            var student = await _studentRepository.GetByIdAsync(application.StudentId)
                ?? throw new Exception("Student not found.");

            application.ApplicationStatus = Domain.Entities.ApplicationStatus.Submitted;
            application.ApplicationDate = DateTime.UtcNow;

            await _applicationRepository.UpdateAsync(application);

            return new SubmitApplicationResponse
            {
                Message = $"Application {application.Id} submitted successfully by student {student.FullName}.",
                SubmittedAt = application.ApplicationDate
            };
        }
    }
}
