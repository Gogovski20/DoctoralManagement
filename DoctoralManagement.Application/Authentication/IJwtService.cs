using DoctoralManagement.Application.Dtos;

namespace DoctoralManagement.Application.Authentication
{
    public interface IJwtService
    {
        string GenerateTokenAsync(UserDto user);
    }
}
