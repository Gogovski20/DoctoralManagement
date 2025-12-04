using DoctoralManagement.Domain.Entities;
using MediatR;

namespace DoctoralManagement.Application.Authentication.Queries
{
    public class GetUserByIdQuery : IRequest<GetUserByIdResponse>
    {
        public int UserId { get; set; }
    }

    public class GetUserByIdResponse
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public UserRole Role { get; set; }
    }
}
