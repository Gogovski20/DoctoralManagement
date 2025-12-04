using DoctoralManagement.Application.Dtos;

namespace DoctoralManagement.Application.Common
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsers();
        Task<UserDto?> GetUserById(int id);
    }
}
