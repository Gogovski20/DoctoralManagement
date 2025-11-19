using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.ConferenceParticipations.Queries
{
    public class GetStudentConferencesHandler : IRequestHandler<GetStudentConferencesQuery, IEnumerable<ConferenceParticipationResponse>>
    {
        private readonly IConferenceParticipationRepository _conferenceParticipationRepository;

        public GetStudentConferencesHandler(IConferenceParticipationRepository conferenceParticipationRepository)
        {
            _conferenceParticipationRepository = conferenceParticipationRepository;
        }

        public async Task<IEnumerable<ConferenceParticipationResponse>> Handle(GetStudentConferencesQuery request, CancellationToken cancellationToken)
        {
            var conferences = await _conferenceParticipationRepository.GetByStudentIdAsync(request.StudentId);

            return conferences.Select(c => new ConferenceParticipationResponse 
            {
                Id = c.Id,
                ConferenceName = c.ConferenceName,
                Date = c.Date,
                Role = c.Role,
                IsInternational = c.IsInternational
            }).ToList();
        }
    }
}
