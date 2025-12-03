using DoctoralManagement.Application.Common;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using System.Net;

namespace DoctoralManagement.Application.Applications.Commands
{
    public class SubmitApplicationHandler : IRequestHandler<SubmitApplicationCommand, SubmitApplicationResponse>
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IApplicationDocumentRepository _applicationDocumentRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;

        public SubmitApplicationHandler(IApplicationRepository applicationRepository, IStudentRepository studentRepository, IApplicationDocumentRepository applicationDocumentRepository, ICurrentUserService currentUserService, IAuthService authService)
        {
            _applicationRepository = applicationRepository;
            _studentRepository = studentRepository;
            _applicationDocumentRepository = applicationDocumentRepository;
            _currentUserService = currentUserService;
            _authService = authService;
        }

        public async Task<SubmitApplicationResponse> Handle(SubmitApplicationCommand request, CancellationToken cancellationToken)
        {
            var application = await _applicationRepository.GetByIdAsync(request.ApplicationId)
                ?? throw new DoctoralManagementException("Application not found.", HttpStatusCode.NotFound);

            var currentUserId = _currentUserService.UserId;
            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);

            if (linkedStudentId == null || linkedStudentId != application.StudentId)
            {
                throw new DoctoralManagementException(
                    "You can only submit your own application.",
                    HttpStatusCode.Forbidden);
            }

            if (application.ApplicationStatus != Domain.Entities.ApplicationStatus.Draft)
            {
                throw new DoctoralManagementException("Only draft applications can be submitted.", HttpStatusCode.BadRequest);
            }

            var hasRequiredDocuments = await _applicationDocumentRepository.HasAllRequiredDocumentsAsync(application.Id);
            if (!hasRequiredDocuments)
            {
                throw new DoctoralManagementException("Cannot submit application. Required documents are missing.", HttpStatusCode.BadRequest);
            }

            var student = await _studentRepository.GetByIdAsync(application.StudentId)
                ?? throw new DoctoralManagementException("Student not found.", HttpStatusCode.NotFound);

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
