using MediatR;

namespace DoctoralManagement.Application.Mentors.Queries
{
    public class GetAllMentorsQuery : IRequest<IEnumerable<GetMentorResponse>>
    {
    }
}
