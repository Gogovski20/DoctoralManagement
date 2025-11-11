using MediatR;

namespace DoctoralManagement.Application.Mentors.Queries
{
    public class GetMentorByIdQuery : IRequest<GetMentorResponse>
    {
        public int Id { get; set; }
    }
}
