using DoctoralManagement.Domain.Entities;

namespace DoctoralManagement.Domain.Interfaces
{
    public interface IApplicationRepository
    {
        Task<Application> GetByIdAsync(int id);
        Task<IEnumerable<Application>> GetAllAsync();
        Task<IEnumerable<Application>> GetByStudentIdAsync(int studentId);
        Task<IEnumerable<Application>> GetByProgramIdAsync(int programId);
        Task<IEnumerable<Application>> GetByStatusAsync(ApplicationStatus status);
        Task<Application> AddAsync(Application application);
        Task UpdateAsync(Application application);
        Task DeleteAsync(Application application);
        Task<bool> HasActiveApplicationAsync(int studentId, int programId);
    }
}
