using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.Dtos;
using Microsoft.AspNetCore.Identity;

namespace DoctoralManagement.Infrastructure.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<int> CreateUserAsync(RegisterUserDto userDto, string password)
        {
            var user = new ApplicationUser
            {
                Email = userDto.Email,
                UserName = userDto.Email,
                FullName = userDto.FullName,
                Role = userDto.Role
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            return user.Id;
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }

        public async Task<UserDto?> ValidateUserAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
                return null;

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                Role = user.Role,
                IsActive = user.IsActive
            };
        }
    }
}
