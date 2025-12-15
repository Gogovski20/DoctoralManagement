using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.Dtos;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using System.Net;

namespace DoctoralManagement.Application.ConferenceParticipations.Queries
{
    public class GetStudentConferencesHandler : IRequestHandler<GetStudentConferencesQuery, IEnumerable<ConferenceParticipationResponse>>
    {
        private readonly IConferenceParticipationRepository _conferenceParticipationRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;

        public GetStudentConferencesHandler(IConferenceParticipationRepository conferenceParticipationRepository, ICurrentUserService currentUserService, IAuthService authService)
        {
            _conferenceParticipationRepository = conferenceParticipationRepository;
            _currentUserService = currentUserService;
            _authService = authService;
        }

        public async Task<IEnumerable<ConferenceParticipationResponse>> Handle(GetStudentConferencesQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            var currentUserRole = _currentUserService.Role;
            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);

            bool isOwner = linkedStudentId == request.StudentId;
            bool isAdmin = currentUserRole == "Admin";

            if (!isOwner && !isAdmin)
            {
                throw new DoctoralManagementException(
                    "You can only view your own conference participations.",
                    HttpStatusCode.Forbidden);
            }

            var conferences = await _conferenceParticipationRepository.GetByStudentIdAsync(request.StudentId);

            return conferences.Select(c => new ConferenceParticipationResponse 
            {
                Id = c.Id,
                ConferenceName = c.ConferenceName,
                Date = c.Date,
                Role = c.Role,
                IsInternational = c.IsInternational,
                Document = c.Document == null ? null : new DocumentDto
                {
                    Id = c.Document.Id,
                    FileName = c.Document.FileName
                }
            }).ToList();
        }
    }
}
