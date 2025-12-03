using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.Dtos;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DoctoralManagement.Application.ConferenceParticipations.Queries
{
    public class GetConferenceParticipationByIdHandler : IRequestHandler<GetConferenceParticipationByIdQuery, GetConferenceParticipationByIdResponse>
    {
        private readonly IConferenceParticipationRepository _conferenceParticipationRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;
        private readonly ILogger<GetConferenceParticipationByIdHandler> _logger;

        public GetConferenceParticipationByIdHandler(IConferenceParticipationRepository conferenceParticipationRepository, ICurrentUserService currentUserService, IAuthService authService, ILogger<GetConferenceParticipationByIdHandler> logger)
        {
            _conferenceParticipationRepository = conferenceParticipationRepository;
            _currentUserService = currentUserService;
            _authService = authService;
            _logger = logger;
        }

        public async Task<GetConferenceParticipationByIdResponse> Handle(GetConferenceParticipationByIdQuery request, CancellationToken cancellationToken)
        {
            var conference = await _conferenceParticipationRepository.GetByIdAsync(request.ConferenceId)
                ?? throw new DoctoralManagementException("Conference not found", HttpStatusCode.NotFound);

            var currentUserId = _currentUserService.UserId;
            var currentUserRole = _currentUserService.Role;
            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);
            var linkedMentorId = await _authService.GetLinkedMentorIdAsync(currentUserId);

            bool isOwner = linkedStudentId == conference.StudentId;
            bool isAdmin = currentUserRole == "Admin";
            bool isMentor = linkedMentorId.HasValue;

            if (!isOwner && !isAdmin && !isMentor)
            {
                throw new DoctoralManagementException(
                    "You can only view your own conference participations.",
                    HttpStatusCode.Forbidden);
            }

            _logger.LogInformation(
               "{Role} {UserId} viewed conference participation {ConferenceId}",
               currentUserRole, currentUserId, request.ConferenceId);

            ActivityDocumentDto? documentDto = null;

            if (conference.Document != null)
            {
                documentDto = new ActivityDocumentDto
                {
                    Id = conference.Document.Id,
                    Type = conference.Document.DocumentType,
                    FileName = conference.Document.FileName,
                    FilePath = conference.Document.FilePath,
                    ContentType = conference.Document.ContentType,
                    UploadedAt = conference.Document.UploadedAt,
                    ReviewComment = conference.Document.ReviewComment ?? "N/A",
                    ReviewedBy = conference.Document.ReviewedBy.HasValue ? (int)conference.Document.ReviewedBy.Value : 0,
                    ReviewedAt = conference.Document.ReviewedAt ?? DateTime.MinValue
                };
            }

            return new GetConferenceParticipationByIdResponse
            {
                Id = conference.Id,
                StudentName = conference.Student?.FullName ?? "N/A",
                ConferenceName = conference.ConferenceName,
                Date = conference.Date,
                Role = conference.Role,
                IsInternational = conference.IsInternational,
                Document = documentDto
            };
        }
    }
}
