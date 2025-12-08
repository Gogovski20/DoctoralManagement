using DoctoralManagement.Domain.Entities;

namespace DoctoralManagement.Domain.Interfaces
{
    public interface IMentorRepository
    {
        Task<Mentor?> GetByIdAsync(int id);
        Task<Mentor?> GetByEmailAsync(string email);
        Task<Mentor?> GetByUserIdAsync(int UserId);
        Task<IEnumerable<Mentor>> GetAllAsync();
        Task<Mentor> AddAsync(Mentor mentor);
        Task UpdateAsync(Mentor mentor);
        Task DeleteAsync(int id);
        Task<bool> IsAvailableForNewStudentAsync(int mentorId);
    }
}
