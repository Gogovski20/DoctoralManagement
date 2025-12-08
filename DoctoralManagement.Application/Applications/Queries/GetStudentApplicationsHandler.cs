using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Applications.Queries
{
    public class GetStudentApplicationsHandler : IRequestHandler<GetStudentApplicationsQuery, IEnumerable<GetStudentApplicationsResponse>>
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IDoctoralProgramRepository _doctoralProgramRepository;

        public GetStudentApplicationsHandler(IApplicationRepository applicationRepository, IDoctoralProgramRepository doctoralProgramRepository)
        {
            _applicationRepository = applicationRepository;
            _doctoralProgramRepository = doctoralProgramRepository;
        }

        public async Task<IEnumerable<GetStudentApplicationsResponse>> Handle(GetStudentApplicationsQuery request, CancellationToken cancellationToken)
        {
            var applications = await _applicationRepository.GetByStudentIdAsync(request.StudentId);

            var responses = new List<GetStudentApplicationsResponse>();

            foreach (var application in applications)
            {
                // Get the program information separately to ensure it's loaded
                var program = await _doctoralProgramRepository.GetByIdAsync(application.DoctoralProgramId);

                responses.Add(new GetStudentApplicationsResponse
                {
                    Id = application.Id,
                    DoctoralProgramId = application.DoctoralProgramId,
                    ProgramName = program?.Name ?? "Unknown Program",
                    Faculty = program?.Faculty ?? "Unknown Faculty",
                    ApplicationStatus = application.ApplicationStatus,
                    ApplicationDate = application.ApplicationDate,
                    DecisionDate = application.DecisionDate
                });
            }

            return responses;
        }
    }
}
