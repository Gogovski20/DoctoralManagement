using DoctoralManagement.Application.Common;
using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DoctoralManagement.Application.DoctoralProjects.Commands
{
    public class SubmitDoctoralProjectHandler : IRequestHandler<SubmitDoctoralProjectCommand, SubmitDoctoralProjectResponse>
    {
        private readonly IDoctoralProjectRepository _doctoralProjectRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;
        private readonly ILogger<SubmitDoctoralProjectHandler> _logger;

        public SubmitDoctoralProjectHandler(IDoctoralProjectRepository doctoralProjectRepository, IStudentRepository studentRepository, IApplicationRepository applicationRepository, ICurrentUserService currentUserService, IAuthService authService, ILogger<SubmitDoctoralProjectHandler> logger)
        {
            _doctoralProjectRepository = doctoralProjectRepository;
            _studentRepository = studentRepository;
            _applicationRepository = applicationRepository;
            _currentUserService = currentUserService;
            _authService = authService;
            _logger = logger;
        }

        public async Task<SubmitDoctoralProjectResponse> Handle(SubmitDoctoralProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _doctoralProjectRepository.GetByIdAsync(request.ProjectId)
                ?? throw new DoctoralManagementException($"Doctoral project with id {request.ProjectId} not found", HttpStatusCode.NotFound);

            var currentUserId = _currentUserService.UserId;
            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);

            if (linkedStudentId == null || linkedStudentId != project.StudentId)
            {
                throw new DoctoralManagementException(
                    "You can only submit your own doctoral projects.",
                    HttpStatusCode.Forbidden);
            }

            if (project.Status != ProjectStatus.Draft && project.Status != ProjectStatus.ChangesRequested)
            {
                throw new DoctoralManagementException("Only Draft or ChangesRequested projects can be sumbitted", HttpStatusCode.BadRequest);
            }

            var student = await _studentRepository.GetByIdAsync(project.StudentId)
                ?? throw new DoctoralManagementException("Student not found", HttpStatusCode.NotFound);

            var hasAccepted = await _applicationRepository.HasFinalAcceptedApplicationAsync(project.StudentId);
            if (!hasAccepted)
            {
                throw new DoctoralManagementException("Student is not admitted to a doctoral program", HttpStatusCode.BadRequest);
            }

            var hasProposal = project.Documents?.Any(d => d.DocumentType == ActivityDocumentType.DoctoralProjectReport) ?? false;
            if (!hasProposal)
            {
                throw new DoctoralManagementException("Doctoral project proposal document is required for submission", HttpStatusCode.BadRequest);
            }


            project.Status = ProjectStatus.Submitted;
            project.SubmittedAt = DateTime.UtcNow;

            await _doctoralProjectRepository.UpdateAsync(project);

            return new SubmitDoctoralProjectResponse
            {
                Id = project.Id,
                Title = project.Title,
                ResearchArea = project.ResearchArea,
                Status = project.Status.ToString(),
                SubmittedAt = project.SubmittedAt.Value
            };
        }
    }
}
