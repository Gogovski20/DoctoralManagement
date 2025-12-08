using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Applications.Queries
{
    public class GetApplicationsByMentorHandler : IRequestHandler<GetApplicationsByMentorQuery, IEnumerable<GetApplicationsByMentorResponse>>
    {
        private readonly IApplicationRepository _applicationRepository;

        public GetApplicationsByMentorHandler(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task<IEnumerable<GetApplicationsByMentorResponse>> Handle(GetApplicationsByMentorQuery request, CancellationToken cancellationToken)
        {
            var applications = await _applicationRepository.GetByMentorIdAsync(request.PreferredMentorId);

            return applications.Select(application => new GetApplicationsByMentorResponse
            {
                Id = application.Id,
                DoctoralProgramId = application.DoctoralProgramId,
                MentorName = application.PrefferedMentor?.FullName ?? "N/A",
                ProgramName = application.DoctoralProgram.Name,
                Faculty = application.DoctoralProgram.Faculty,
                ApplicationStatus = application.ApplicationStatus,
                ApplicationDate = application.ApplicationDate,
                DecisionDate = application.DecisionDate,
            });
        }
    }
}
