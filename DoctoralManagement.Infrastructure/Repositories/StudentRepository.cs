using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using DoctoralManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DoctoralManagement.Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Student> AddAsync(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task DeleteAsync(Student student)
        {
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Students.AnyAsync(s => s.Email == email);
        }

        public async Task<bool> ExistsByIndexNumberAsync(string indexNumber)
        {
            return await _context.Students.AnyAsync(s => s.IndexNumber == indexNumber);
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            return await _context.Students
                .ToListAsync();
        }

        public async Task<IEnumerable<Student>> GetAllNoTrackingAsync()
        {
            return await _context.Students
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Student>> GetAllWithProgramAsync()
        {
            return await _context.Students
                .Include(s => s.DoctoralProgram)
                .ToListAsync();
        }

        public async Task<IEnumerable<Student>> GetAllWithProgramNoTrackingAsync()
        {
            return await _context.Students
                .Include(s => s.DoctoralProgram)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Student> GetByEmailAsync(string email)
        {
            return await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Email == email);
        }

        public async Task<Student> GetByIdAsync(int id)
        {
            return await _context.Students.FindAsync(id);
        }

        public async Task<Student> GetByIdNoTrackingAsync(int id)
        {
            return await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Student> GetByIdWithProgramAsync(int id)
        {
            return await _context.Students
                .Include(s => s.DoctoralProgram)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Student> GetByIdWithProgramNoTrackingAsync(int id)
        {
            return await _context.Students
                .Include(s => s.DoctoralProgram)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Student> GetByIndexNumberAsync(string indexNumber)
        {
            return await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.IndexNumber == indexNumber);
        }

        public async Task UpdateAsync(Student student)
        {
            var existing = await _context.Students.FindAsync(student.Id);
            if (existing != null)
            {
                existing.FullName = student.FullName;
                existing.Email = student.Email;
                existing.IndexNumber = student.IndexNumber;
                existing.EnrollmentDate = student.EnrollmentDate;
                existing.GPA = student.GPA;
                existing.EnglishCertificate = student.EnglishCertificate;
                existing.Status = student.Status;
                existing.TotalCreditsFromBachelor = student.TotalCreditsFromBachelor;
                existing.TotalCreditsFromMaster = student.TotalCreditsFromMaster;
                existing.DoctoralProgramId = student.DoctoralProgramId;

                await _context.SaveChangesAsync();
            }
        }
    }
}
