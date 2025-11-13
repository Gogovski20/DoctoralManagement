using MediatR;

namespace DoctoralManagement.Application.DoctoralProjects.Queries
{
    public class GetDoctoralProjectsByMentorQuery : IRequest<IEnumerable<GetDoctoralProjectResponse>>
    {
        public int MentorId { get; set; }
        
        public GetDoctoralProjectsByMentorQuery(int mentorId)
        {
            MentorId = mentorId;
        }
    }
}
