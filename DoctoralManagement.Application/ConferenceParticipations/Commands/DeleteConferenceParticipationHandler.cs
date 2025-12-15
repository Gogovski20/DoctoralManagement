using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.ECTS.Services;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using System.Net;

namespace DoctoralManagement.Application.ConferenceParticipations.Commands
{
    public class DeleteConferenceParticipationHandler : IRequestHandler<DeleteConferenceParticipationCommand, bool>
    {
        private readonly IConferenceParticipationRepository _conferenceParticipationRepository;
        private readonly IEctsTrackingRepository _ectsTrackingRepository;
        private readonly EctsProgressService _ectsProgressService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;

        public DeleteConferenceParticipationHandler(IConferenceParticipationRepository conferenceParticipationRepository, IEctsTrackingRepository ectsTrackingRepository, EctsProgressService ectsProgressService, ICurrentUserService currentUserService, IAuthService authService)
        {
            _conferenceParticipationRepository = conferenceParticipationRepository;
            _ectsTrackingRepository = ectsTrackingRepository;
            _ectsProgressService = ectsProgressService;
            _currentUserService = currentUserService;
            _authService = authService;
        }

        public async Task<bool> Handle(DeleteConferenceParticipationCommand request, CancellationToken cancellationToken)
        {
            var conference = await _conferenceParticipationRepository.GetByIdAsync(request.Id)
                ?? throw new Exception($"Conference with id {request.Id} not found");

            if (conference.IsApproved == true)
            {
                throw new DoctoralManagementException("You can't delete approved conferences.", HttpStatusCode.BadRequest);
            }

            var currentUserId = _currentUserService.UserId;

            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);

            if (linkedStudentId == null || linkedStudentId != conference.StudentId)
            {
                throw new DoctoralManagementException("You can only delete conference participation for your own account.", HttpStatusCode.Forbidden);
            }

            await _conferenceParticipationRepository.DeleteAsync(conference.Id);

            return true;
        }
    }
}
