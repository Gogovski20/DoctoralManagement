using DoctoralManagement.Domain.Entities;
using MediatR;

namespace DoctoralManagement.Application.Applications.Queries
{
    public class GetProgramApplicationsQuery : IRequest<IEnumerable<GetProgramApplicationsResponse>>
    {
        public int ProgramId { get; set; }
        public ApplicationStatus? Status { get; set; }
    }
}
