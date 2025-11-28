using DoctoralManagement.Application.Common;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DoctoralManagement.Infrastructure.Auth
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdClaim, out var id))
                {
                    throw new UnauthorizedAccessException("User ID not found in token.");
                }
                return id;
            }
        }

        public string? Role =>
            _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);

        public string? Email =>
            _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

        public bool CanAccessStudent(int studentId)
        {
            bool isOwner = UserId == studentId;
            bool isPrivileged = Role is "Secretary" or "Mentor" or "Committee" or "Admin";

            return isOwner || isPrivileged;
        }
    }
}
