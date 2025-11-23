using DoctoralManagement.Application.Authentication.Commands;
using DoctoralManagement.Application.Dtos;

namespace DoctoralManagement.Application.Common
{
    public interface IAuthService
    {
        Task<bool> UserExistsAsync(string email);
        Task<int> CreateUserAsync(RegisterUserDto user, string password);
        Task<UserDto?> ValidateUserAsync(string email, string password);
    }
}
