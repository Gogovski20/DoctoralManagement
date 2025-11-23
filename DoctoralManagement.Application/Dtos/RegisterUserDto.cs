using DoctoralManagement.Domain.Entities;

namespace DoctoralManagement.Application.Dtos
{
    public class RegisterUserDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }

        public RegisterUserDto(string fullName, string email, UserRole role)
        {
            FullName = fullName;
            Email = email;
            Role = role;
        }
    }
}
