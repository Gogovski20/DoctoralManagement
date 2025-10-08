using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Applications.Queries
{
    public class GetAllApplicationsHandler : IRequestHandler<GetAllApplicationsQuery, IEnumerable<GetAllApplicationResponse>>
    {
        private readonly IApplicationRepository _applicationRepository;

        public GetAllApplicationsHandler(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task<IEnumerable<GetAllApplicationResponse>> Handle(GetAllApplicationsQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Domain.Entities.Application> applications;

            if (request.StudentId.HasValue && request.ProgramId.HasValue)
            {
                var studentApps = await _applicationRepository.GetByStudentIdAsync(request.StudentId.Value);
                applications = studentApps.Where(a => a.DoctoralProgramId == request.ProgramId.Value);
            }
            else if (request.StudentId.HasValue) 
            {
                applications = await _applicationRepository.GetByStudentIdAsync(request.StudentId.Value);
            }
            else if (request.ProgramId.HasValue)
            {
                applications = await _applicationRepository.GetByProgramIdAsync(request.ProgramId.Value);
            }
            else if (request.Status.HasValue)
            {
                applications = await _applicationRepository.GetByStatusAsync(request.Status.Value);
            }
            else
            {
                applications = await _applicationRepository.GetAllAsync();
            }

            if (request.Status.HasValue)
            {
                applications = applications.Where(a => a.ApplicationStatus == request.Status.Value);
            }

            return applications.Select(a => new GetAllApplicationResponse
            {
                Id = a.Id,
                StudentId = a.StudentId,
                StudentName = a.Student.FullName,
                StudentEmail = a.Student.Email,
                DoctoralProgramId = a.DoctoralProgramId,
                ProgramName = a.DoctoralProgram.Name,
                ScientificArea = a.DoctoralProgram.ScientificArea,
                ApplicationStatus = a.ApplicationStatus,
                ApplicationDate = a.ApplicationDate,
                DecisionDate = a.DecisionDate,
                MeetsGradeRequirements = a.MeetsGradeRequirements
            });
        }
    }
}
