using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Applications.Queries
{
    public class GetStudentApplicationsHandler : IRequestHandler<GetStudentApplicationsQuery, IEnumerable<GetStudentApplicationsResponse>>
    {
        private readonly IApplicationRepository _applicationRepository;

        public GetStudentApplicationsHandler(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task<IEnumerable<GetStudentApplicationsResponse>> Handle(GetStudentApplicationsQuery request, CancellationToken cancellationToken)
        {
            var applications = await _applicationRepository.GetByStudentIdAsync(request.StudentId);

            return applications.Select(a => new GetStudentApplicationsResponse
            {
                Id = a.Id,
                DoctoralProgramId = a.DoctoralProgramId,
                ProgramName = a.DoctoralProgram.Name,
                Faculty = a.DoctoralProgram.Faculty,
                ApplicationStatus = a.ApplicationStatus,
                ApplicationDate = a.ApplicationDate,
                DecisionDate = a.DecisionDate
            });
        }
    }
}
