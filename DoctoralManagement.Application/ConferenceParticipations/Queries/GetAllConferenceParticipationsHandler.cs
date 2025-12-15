using DoctoralManagement.Application.Dtos;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.ConferenceParticipations.Queries
{
    public class GetAllConferenceParticipationsHandler : IRequestHandler<GetAllConferenceParticipationsQuery, IEnumerable<GetAllConferenceParticipationsResponse>>
    {
        private readonly IConferenceParticipationRepository _conferenceParticipationRepository;

        public GetAllConferenceParticipationsHandler(IConferenceParticipationRepository conferenceParticipationRepository)
        {
            _conferenceParticipationRepository = conferenceParticipationRepository;
        }

        public async Task<IEnumerable<GetAllConferenceParticipationsResponse>> Handle(GetAllConferenceParticipationsQuery request, CancellationToken cancellationToken)
        {
            var conferences = await _conferenceParticipationRepository.GetAllAsync();

            return conferences.Select(c => new GetAllConferenceParticipationsResponse
            {
                Id = c.Id,
                StudentName = c.Student?.FullName ?? "N/A",
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
