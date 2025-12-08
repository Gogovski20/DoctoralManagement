using DoctoralManagement.Application.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace DoctoralManagement.Infrastructure.Auth
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CurrentUserService> _logger;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, ILogger<CurrentUserService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public int UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                _logger.LogInformation($"User ID claim from JWT: {userIdClaim}");

                if (int.TryParse(userIdClaim, out var id))
                {
                    _logger.LogInformation($"Parsed User ID: {id}");
                    return id;
                }

                _logger.LogWarning("Could not parse User ID from JWT");
                throw new UnauthorizedAccessException("User ID not found in token.");
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
