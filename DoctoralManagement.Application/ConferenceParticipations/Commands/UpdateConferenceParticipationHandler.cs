using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.ECTS.Services;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using System.Net;

namespace DoctoralManagement.Application.ConferenceParticipations.Commands
{
    public class UpdateConferenceParticipationHandler : IRequestHandler<UpdateConferenceParticipationCommand, UpdateConferenceParticipationResponse>
    {
        private readonly IConferenceParticipationRepository _conferenceParticipationRepository;
        private readonly IEctsTrackingRepository _ectsTrackingRepository;
        private readonly EctsProgressService _ectsProgressService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;
        public UpdateConferenceParticipationHandler(IConferenceParticipationRepository conferenceParticipationRepository, IEctsTrackingRepository ectsTrackingRepository, EctsProgressService ectsProgressService, ICurrentUserService currentUserService, IAuthService authService)
        {
            _conferenceParticipationRepository = conferenceParticipationRepository;
            _ectsTrackingRepository = ectsTrackingRepository;
            _ectsProgressService = ectsProgressService;
            _currentUserService = currentUserService;
            _authService = authService;
        }

        public async Task<UpdateConferenceParticipationResponse> Handle(UpdateConferenceParticipationCommand request, CancellationToken cancellationToken)
        {
            var conference = await _conferenceParticipationRepository.GetByIdAsync(request.Id)
                ?? throw new DoctoralManagementException($"Conference with id {request.Id} not found", HttpStatusCode.NotFound);

            var currentUserId = _currentUserService.UserId;
            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);

            if (linkedStudentId == null || linkedStudentId != conference.StudentId)
            {
                throw new DoctoralManagementException(
                    "You can only update your own conference participations.",
                    HttpStatusCode.Forbidden);
            }

            if (conference.IsApproved)
            {
                throw new DoctoralManagementException(
                    "Cannot update an already approved conference participation.",
                    HttpStatusCode.BadRequest);
            }

            var dateUtc = DateTime.SpecifyKind(request.Date, DateTimeKind.Utc);

            conference.ConferenceName = request.ConferenceName;
            conference.Date = dateUtc;
            conference.Role = request.Role;
            conference.IsInternational = request.IsInternational;

            await _conferenceParticipationRepository.UpdateAsync(conference);

            return new UpdateConferenceParticipationResponse { };
        }
    }
}
