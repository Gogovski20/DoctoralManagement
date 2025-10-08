using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Applications.Queries
{
    public class GetProgramApplicationsHandler : IRequestHandler<GetProgramApplicationsQuery, IEnumerable<GetProgramApplicationsResponse>>
    {
        private readonly IApplicationRepository _applicationRepository;

        public GetProgramApplicationsHandler(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task<IEnumerable<GetProgramApplicationsResponse>> Handle(GetProgramApplicationsQuery request, CancellationToken cancellationToken)
        {
            var applications = await _applicationRepository.GetByProgramIdAsync(request.ProgramId);

            if (request.Status.HasValue)
            {
                applications = applications.Where(a => a.ApplicationStatus == request.Status.Value);
            }

            return applications.Select(a => new GetProgramApplicationsResponse
            {
                Id = a.Id,
                StudentId = a.StudentId,
                StudentName = a.Student.FullName,
                StudentEmail = a.Student.Email,
                StudentGPA = a.Student.GPA,
                ApplicationStatus = a.ApplicationStatus,
                ApplicationDate = a.ApplicationDate,
                MeetsGradeRequirements = a.MeetsGradeRequirements
            });
        }
    }
}
