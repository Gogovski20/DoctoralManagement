using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.Dtos;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DoctoralManagement.Application.Publications.Queries
{
    public class GetPublicationByIdHandler : IRequestHandler<GetPublicationByIdQuery, GetPublicationByIdResponse>
    {
        private readonly IPublicationRepository _publicationRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;
        private readonly ILogger<GetPublicationByIdHandler> _logger;

        public GetPublicationByIdHandler(IPublicationRepository publicationRepository, ICurrentUserService currentUserService, IAuthService authService, ILogger<GetPublicationByIdHandler> logger)
        {
            _publicationRepository = publicationRepository;
            _currentUserService = currentUserService;
            _authService = authService;
            _logger = logger;
        }

        public async Task<GetPublicationByIdResponse> Handle(GetPublicationByIdQuery request, CancellationToken cancellationToken)
        {
            var publication = await _publicationRepository.GetByIdAsync(request.PublicationId)
                ?? throw new DoctoralManagementException("Publication not found.", HttpStatusCode.NotFound);

            var currentUserId = _currentUserService.UserId;
            var currentUserRole = _currentUserService.Role;
            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);
            var linkedMentorId = await _authService.GetLinkedMentorIdAsync(currentUserId);

            bool isOwner = linkedStudentId == publication.StudentId;
            bool isAdmin = currentUserRole == "Admin";
            bool isMentor = linkedMentorId.HasValue;

            if (!isOwner && !isAdmin && !isMentor)
            {
                throw new DoctoralManagementException(
                    "You can only view your own publications.",
                    HttpStatusCode.Forbidden);
            }

            _logger.LogInformation(
               "{Role} {UserId} viewed publication {PublicationId}",
               currentUserRole, currentUserId, request.PublicationId);

            ActivityDocumentDto? documentDto = null;

            if (publication.Document != null)
            {
                documentDto = new ActivityDocumentDto
                {
                    Id = publication.Document.Id,
                    Type = publication.Document.DocumentType,
                    FileName = publication.Document.FileName,
                    FilePath = publication.Document.FilePath,
                    ContentType = publication.Document.ContentType,
                    UploadedAt = publication.Document.UploadedAt,
                    ReviewComment = publication.Document.ReviewComment ?? "N/A",
                    ReviewedBy = publication.Document.ReviewedBy.HasValue ? (int)publication.Document.ReviewedBy.Value : 0,
                    ReviewedAt = publication.Document.ReviewedAt ?? DateTime.MinValue
                };
            }

            return new GetPublicationByIdResponse
            {
                Id = publication.Id,
                StudentName = publication.Student?.FullName ?? "N/A",
                Title = publication.Title,
                Journal = publication.Journal,
                PublishedOn = publication.PublishedOn,
                IsIndexedInScopus = publication.IsIndexedInScopus,
                IsIndexedInThomsonReuters = publication.IsIndexedInThomsonReuters,
                IsApproved = publication.IsApproved,
                Document = documentDto
            };
        }
    }
}
