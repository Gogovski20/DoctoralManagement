using DoctoralManagement.Domain.Entities;

namespace DoctoralManagement.Domain.Interfaces
{
    public interface IThesisDefenseRepository
    {
        Task<ThesisDefense> AddAsync(ThesisDefense defense);
        Task<ThesisDefense?> GetByProjectIdAsync(int projectId);
        Task<bool> ExistsForProjectAsync(int projectId);
    }
}
