using DoctoralManagement.Domain.Entities;

namespace DoctoralManagement.Domain.Interfaces
{
    public interface IDoctoralProjectRepository
    {
        Task<DoctoralProject?> GetByIdAsync(int id);
        Task<IEnumerable<DoctoralProject>> GetByStudentIdAsync(int studentId);
        Task<DoctoralProject> AddAsync(DoctoralProject project);
        Task UpdateAsync(DoctoralProject project);
        Task<bool> ExistsActiveProjectForStudentAsync(int studentId);
        Task<IEnumerable<DoctoralProject>> GetAllWithDetailsAsync();
        Task<IEnumerable<DoctoralProject>> GetByMentorIdAsync(int mentorId);
    }
}
