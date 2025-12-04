using DoctoralManagement.Domain.Entities;
using MediatR;

namespace DoctoralManagement.Application.Authentication.Queries
{
    public class GetAllUsersQuery : IRequest<IEnumerable<GetAllUsersResponse>>
    {
    }

    public class GetAllUsersResponse
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
    }
}
