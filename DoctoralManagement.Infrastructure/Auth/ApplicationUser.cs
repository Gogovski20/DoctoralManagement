using DoctoralManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DoctoralManagement.Infrastructure.Auth
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FullName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }

        public int? StudentId { get; set; }
        

        public int? MentorId { get; set; }
        
    }
}
