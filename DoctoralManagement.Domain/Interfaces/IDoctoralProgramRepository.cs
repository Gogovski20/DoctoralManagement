using DoctoralManagement.Domain.Entities;

namespace DoctoralManagement.Domain.Interfaces
{
    public interface IDoctoralProgramRepository
    {
        Task<DoctoralProgram> GetByIdAsync(int id);
        Task<DoctoralProgram> GetByNameAsync(string name);
        Task<IEnumerable<DoctoralProgram>> GetAllAsync();
        Task<IEnumerable<DoctoralProgram>> GetByScientificAreaAsync(string scientificArea);
        Task<DoctoralProgram> AddAsync(DoctoralProgram doctoralProgram);
        Task UpdateAsync(DoctoralProgram doctoralProgram);
        Task DeleteAsync(DoctoralProgram doctoralProgram);
        Task<bool> ExistsByNameAsync(string name);
    }
}
