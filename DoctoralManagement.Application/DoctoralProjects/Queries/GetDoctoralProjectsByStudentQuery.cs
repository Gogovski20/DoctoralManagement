using MediatR;

namespace DoctoralManagement.Application.DoctoralProjects.Queries
{
    public class GetDoctoralProjectsByStudentQuery : IRequest<IEnumerable<GetDoctoralProjectResponse>>
    {
        public int StudentId { get; set; }
        public GetDoctoralProjectsByStudentQuery(int studentId)
        {
            StudentId = studentId;
        }
    }
}
