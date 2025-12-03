using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.Dtos;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DoctoralManagement.Application.DoctoralProjects.Queries
{
    public class GetDoctoralProjectByIdHandler : IRequestHandler<GetDoctoralProjectByIdQuery, GetDoctoralProjectByIdResponse>
    {
        private readonly IDoctoralProjectRepository _repository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;
        private readonly ILogger<GetDoctoralProjectByIdHandler> _logger;

        public GetDoctoralProjectByIdHandler(IDoctoralProjectRepository repository, ICurrentUserService currentUserService, IAuthService authService, ILogger<GetDoctoralProjectByIdHandler> logger)
        {
            _repository = repository;
            _currentUserService = currentUserService;
            _authService = authService;
            _logger = logger;
        }

        public async Task<GetDoctoralProjectByIdResponse> Handle(GetDoctoralProjectByIdQuery request, CancellationToken cancellationToken)
        {
            var project = await _repository.GetByIdAsync(request.DoctoralProjectId)
                ?? throw new DoctoralManagementException("Project not found", HttpStatusCode.NotFound);

            var currentUserId = _currentUserService.UserId;
            var currentUserRole = _currentUserService.Role;
            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);
            var linkedMentorId = await _authService.GetLinkedMentorIdAsync(currentUserId);

            bool isOwner = linkedStudentId == project.StudentId;
            bool isAdmin = currentUserRole == "Admin";
            bool isMentor = linkedMentorId == project.MentorId;
            bool isCommittee = currentUserRole == "Committee";
            bool isSecretary = currentUserRole == "Secretary";

            if (!isOwner && !isAdmin && !isMentor && !isCommittee && !isSecretary)
            {
                throw new DoctoralManagementException(
                    "You don't have permission to view this doctoral project.",
                    HttpStatusCode.Forbidden);
            }

            _logger.LogInformation(
                "{Role} {UserId} viewed doctoral project {ProjectId}. Student: {StudentId}",
                currentUserRole, currentUserId, request.DoctoralProjectId, project.StudentId);

            var documents = new List<ActivityDocumentDto>();
            if (project.Documents != null && project.Documents.Any())
            {
                documents = project.Documents.Select(d => new ActivityDocumentDto
                {
                    Id = d.Id,
                    Type = d.DocumentType,
                    FileName = d.FileName,
                    FilePath = d.FilePath,
                    ContentType = d.ContentType,
                    UploadedAt = d.UploadedAt,
                    ReviewComment = d.ReviewComment ?? "N/A",
                    ReviewedBy = d.ReviewedBy ?? 0,
                    ReviewedAt = d.ReviewedAt ?? DateTime.MinValue
                }).ToList();
            }

            return new GetDoctoralProjectByIdResponse
            {
                Id = project.Id,
                Title = project.Title,
                ResearchArea = project.ResearchArea,
                Status = project.Status.ToString(),
                StudentName = project.Student?.FullName ?? "N/A",
                MentorName = project.Mentor?.FullName ?? "N/A",
                CreatedAt = project.CreatedAt,
                SubmittedAt = project.SubmittedAt,
                Documents = documents
            };
        }
    }
}
