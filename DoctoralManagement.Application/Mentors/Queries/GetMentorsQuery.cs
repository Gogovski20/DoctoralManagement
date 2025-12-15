using DoctoralManagement.Application.Dtos;
using MediatR;

namespace DoctoralManagement.Application.Mentors.Queries
{
    public class GetMentorsQuery : IRequest<List<MentorLookupDto>>
    {
    }
}
