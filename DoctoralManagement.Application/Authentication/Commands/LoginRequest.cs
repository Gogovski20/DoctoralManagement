using MediatR;

namespace DoctoralManagement.Application.Authentication.Commands
{
    public record LoginRequest(string Email, string Password) : IRequest<LoginResponse>;
}
