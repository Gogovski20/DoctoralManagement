using DoctoralManagement.Domain.Entities;

namespace DoctoralManagement.Domain.Interfaces
{
    public interface IMobilityRepository
    {
        Task<Mobility> AddAsync(Mobility mobility);
        Task<Mobility?> GetByIdAsync(int id);
        Task<IEnumerable<Mobility>> GetByStudentIdAsync(int studentId);
        Task<IEnumerable<Mobility>> GetAllAsync();
        Task UpdateAsync(Mobility mobility);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
