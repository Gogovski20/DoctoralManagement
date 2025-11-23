using DoctoralManagement.Domain.Entities;
using MediatR;

namespace DoctoralManagement.Application.Authentication.Commands
{
    public record RegisterRequest(string FullName, string Email, string Password, UserRole Role) : IRequest<RegisterResponse>;
}
