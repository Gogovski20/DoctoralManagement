using DoctoralManagement.Domain.Entities;
using MediatR;

namespace DoctoralManagement.Application.Applications.Queries
{
    public class GetAllApplicationsQuery : IRequest<IEnumerable<GetAllApplicationResponse>>
    {
        public ApplicationStatus? Status { get; set; }
        public int? ProgramId { get; set; }
        public int? StudentId { get; set; }
    }
}
