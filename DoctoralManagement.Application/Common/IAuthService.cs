using DoctoralManagement.Application.Dtos;

namespace DoctoralManagement.Application.Common
{
    public interface IAuthService
    {
        Task<bool> UserExistsAsync(string email);
        Task<int> CreateUserAsync(RegisterUserDto user, string password, int? studentId = null, int? mentorId = null);
        Task<UserDto?> ValidateUserAsync(string email, string password);
        Task<bool> IsStudentLinkedAsync(int studentId);
        Task<bool> IsMentorLinkedAsync(int mentorId);
        Task<int?> GetLinkedStudentIdAsync(int applicationUserId);
        Task<int?> GetLinkedMentorIdAsync(int applicationUserId);
    }
}
