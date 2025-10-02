using DoctoralManagement.Domain.Entities;

namespace DoctoralManagement.Domain.Interfaces
{
    public interface IStudentRepository
    {
        Task<Student> GetByIdAsync(int id);
        Task<Student> GetByEmailAsync(string email);
        Task<Student> GetByIndexNumberAsync(string indexNumber);
        Task<IEnumerable<Student>> GetAllAsync();
        Task<Student> AddAsync(Student student);
        Task UpdateAsync(Student student);
        Task DeleteAsync(Student student);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByIndexNumberAsync(string indexNumber);
    }
}
