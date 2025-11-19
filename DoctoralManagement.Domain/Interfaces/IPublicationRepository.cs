using DoctoralManagement.Domain.Entities;

namespace DoctoralManagement.Domain.Interfaces
{
    public interface IPublicationRepository
    {
        Task<Publication> AddAsync(Publication publication);
        Task<Publication?> GetByIdAsync(int id);
        Task<IEnumerable<Publication>> GetByStudentIdAsync(int studentId);
        Task<IEnumerable<Publication>> GetAllAsync();
        Task UpdateAsync(Publication publication);
        Task DeleteAsync(int id);
        Task<int> GetPublicationCountByStudentAsync(int studentId);
        Task<bool> ExistsAsync(int id);
    }
}
