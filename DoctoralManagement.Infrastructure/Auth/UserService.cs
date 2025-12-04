using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.Dtos;
using DoctoralManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DoctoralManagement.Infrastructure.Auth
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsers()
        {
            var users = _userManager.Users.ToList();

            var result = new List<UserDto>();

            foreach (var user in users)
            {
                
                result.Add(new UserDto
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    FullName = user.UserName ?? string.Empty,
                    Role = user.Role,
                    IsActive = user.IsActive
                });
            }

            return result;
        }

        public async Task<UserDto?> GetUserById(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return null;
            }

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FullName = user.UserName ?? string.Empty,
                Role = user.Role,
            };
        }
    }
}
