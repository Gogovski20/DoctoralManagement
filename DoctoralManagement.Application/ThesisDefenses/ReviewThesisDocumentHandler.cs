using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.ECTS.Services;
using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DoctoralManagement.Application.ThesisDefenses
{
    public class ReviewThesisDocumentHandler : IRequestHandler<ReviewThesisDocumentCommand, ReviewThesisDocumentResponse>
    {
        private readonly IActivityDocumentRepository _docRepo;
        private readonly IDoctoralProjectRepository _projectRepo;
        private readonly IEctsTrackingRepository _ectsRepo;
        private readonly EctsProgressService _ectsProgressService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<ReviewThesisDocumentHandler> _logger;

        public ReviewThesisDocumentHandler(IActivityDocumentRepository docRepo, IDoctoralProjectRepository projectRepo, IEctsTrackingRepository ectsRepo, EctsProgressService ectsProgressService, ICurrentUserService currentUserService, ILogger<ReviewThesisDocumentHandler> logger)
        {
            _docRepo = docRepo;
            _projectRepo = projectRepo;
            _ectsRepo = ectsRepo;
            _ectsProgressService = ectsProgressService;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<ReviewThesisDocumentResponse> Handle(ReviewThesisDocumentCommand request, CancellationToken cancellationToken)
        {
            var currentUserRole = _currentUserService.Role;
            var allowedRoles = new[] { "Admin", "Mentor", "Committee" };

            if (!allowedRoles.Contains(currentUserRole))
            {
                throw new DoctoralManagementException(
                    "Only admins, mentors, or committee members can review thesis documents.",
                    HttpStatusCode.Forbidden);
            }

            var document = await _docRepo.GetByIdAsync(request.DocumentId)
                ?? throw new DoctoralManagementException("Document not found", HttpStatusCode.NotFound);

            if (document.DocumentType != ActivityDocumentType.DefenseThesisDocument)
            {
                throw new DoctoralManagementException("Invalid document type for thesis defense review", HttpStatusCode.BadRequest);
            }

            var project = await _projectRepo.GetByIdAsync(document.DoctoralProjectId!.Value)
                ?? throw new DoctoralManagementException("Associated doctoral project not found", HttpStatusCode.NotFound);

            document.Status = request.NewStatus;
            document.ReviewComment = request.ReviewComment;
            document.ReviewedAt = DateTime.UtcNow;
            document.ReviewedBy = _currentUserService.UserId;

            int ectsAwarded = 0;
            if (request.NewStatus == DocumentStatus.Approved)
            {
                project.Status = ProjectStatus.DefenseUnderReview;
                var ects = await _ectsRepo.GetByStudentIdAsync(project.StudentId);
                if (ects != null)
                {
                    ects.ThesisDefence += 20;
                    if (ects.ThesisDefence > 46) ects.ThesisDefence = 46;
                    ectsAwarded = 20;

                    await _ectsRepo.UpdateAsync(ects);
                    await _ectsProgressService.UpdateStudentSemesterAsync(project.StudentId, ects.TotalECTS);
                }
            }
            else if (request.NewStatus == DocumentStatus.Rejected)
            {
                project.Status = ProjectStatus.DefenseChangesRequired;
            }

            await _projectRepo.UpdateAsync(project);
            await _docRepo.UpdateAsync(document);

            _logger.LogInformation(
                "Thesis document {DocumentId} reviewed by {Role}. Status: {Status}. ECTS awarded: {ECTS}",
                request.DocumentId, currentUserRole, request.NewStatus, ectsAwarded);

            return new ReviewThesisDocumentResponse
            {
                DocumentId = request.DocumentId,
                DocumentStatus = document.Status.ToString(),
                ProjectStatus = project.Status.ToString(),
                ReviewComment = document.ReviewComment,
                UpdatedECTS = ectsAwarded
            };
        }
    }
}
