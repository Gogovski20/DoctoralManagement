using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.Dtos;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DoctoralManagement.Application.Mobilities.Queries
{
    public class GetMobilityByIdHandler : IRequestHandler<GetMobilityByIdQuery, GetMobilityByIdResponse>
    {
        private readonly IMobilityRepository _mobilityRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;
        private readonly ILogger<GetMobilityByIdHandler> _logger;

        public GetMobilityByIdHandler(IMobilityRepository mobilityRepository, ICurrentUserService currentUserService, IAuthService authService, ILogger<GetMobilityByIdHandler> logger)
        {
            _mobilityRepository = mobilityRepository;
            _currentUserService = currentUserService;
            _authService = authService;
            _logger = logger;
        }

        public async Task<GetMobilityByIdResponse> Handle(GetMobilityByIdQuery request, CancellationToken cancellationToken)
        {
            var mobility = await _mobilityRepository.GetByIdAsync(request.MobilityId)
                ?? throw new DoctoralManagementException("Mobility not found", HttpStatusCode.NotFound);

            var currentUserId = _currentUserService.UserId;
            var currentUserRole = _currentUserService.Role;
            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);
            var linkedMentorId = await _authService.GetLinkedMentorIdAsync(currentUserId);

            bool isOwner = linkedStudentId == mobility.StudentId;
            bool isAdmin = currentUserRole == "Admin";
            bool isMentor = linkedMentorId.HasValue;

            if (!isOwner && !isAdmin && !isMentor)
            {
                throw new DoctoralManagementException(
                    "You can only view your own mobilities.",
                    HttpStatusCode.Forbidden);
            }

            _logger.LogInformation(
               "{Role} {UserId} viewed mobility {MobilityId}",
               currentUserRole, currentUserId, request.MobilityId);

            ActivityDocumentDto? documentDto = null;

            if (mobility.Document != null)
            {
                documentDto = new ActivityDocumentDto
                {
                    Id = mobility.Document.Id,
                    Type = mobility.Document.DocumentType,
                    FileName = mobility.Document.FileName,
                    FilePath = mobility.Document.FilePath,
                    ContentType = mobility.Document.ContentType,
                    UploadedAt = mobility.Document.UploadedAt,
                    ReviewComment = mobility.Document.ReviewComment ?? "N/A",
                    ReviewedBy = mobility.Document.ReviewedBy.HasValue ? (int)mobility.Document.ReviewedBy.Value : 0,
                    ReviewedAt = mobility.Document.ReviewedAt ?? DateTime.MinValue
                };
            }

            return new GetMobilityByIdResponse
            {
                Id = mobility.Id,
                StudentName = mobility.Student?.FullName ?? "N/A",
                Institution = mobility.Institution,
                Country = mobility.Country,
                StartDate = mobility.StartDate,
                EndDate = mobility.EndDate,
                Document = documentDto
            };
        }
    }
}
