using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.Dtos;
using DoctoralManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DoctoralManagement.Infrastructure.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AuthService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<int> CreateUserAsync(RegisterUserDto userDto, string password, int? studentId = null, int? mentorId = null)
        {
            var user = new ApplicationUser
            {
                Email = userDto.Email,
                UserName = userDto.Email,
                FullName = userDto.FullName,
                Role = userDto.Role,
                StudentId = studentId,
                MentorId = mentorId
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, userDto.Role.ToString());

            return user.Id;
        }

        public async Task<int?> GetLinkedMentorIdAsync(int applicationUserId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == applicationUserId);

            return user?.MentorId;
        }

        public async Task<int?> GetLinkedStudentIdAsync(int applicationUserId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == applicationUserId);

            return user?.StudentId;
        }

        public async Task<bool> IsMentorLinkedAsync(int mentorId)
        {
            return await _context.Users.AnyAsync(u => u.MentorId == mentorId);
        }

        public async Task<bool> IsStudentLinkedAsync(int studentId)
        {
            return await _context.Users.AnyAsync(u => u.StudentId == studentId);
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
