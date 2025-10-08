using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using DoctoralManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DoctoralManagement.Infrastructure.Repositories
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Domain.Entities.Application> AddAsync(Domain.Entities.Application application)
        {
            _context.Applications.Add(application);
            await _context.SaveChangesAsync();
            return application;
        }

        public async Task DeleteAsync(Domain.Entities.Application application)
        {
            _context.Applications.Remove(application);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Domain.Entities.Application>> GetAllAsync()
        {
            return await _context.Applications
                .Include(a => a.Student)
                .Include(a => a.DoctoralProgram)
                .Include(a => a.PrefferedMentor)
                .ToListAsync();
        }

        public async Task<Domain.Entities.Application> GetByIdAsync(int id)
        {
            return await _context.Applications
                .Include(a => a.Student)
                .Include(a => a.DoctoralProgram)
                .Include(a => a.PrefferedMentor)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Domain.Entities.Application>> GetByProgramIdAsync(int programId)
        {
            return await _context.Applications
                .Include(a => a.Student)
                .Include(a => a.DoctoralProgram)
                .Include(a => a.PrefferedMentor)
                .Where(a => a.DoctoralProgramId == programId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Domain.Entities.Application>> GetByStatusAsync(ApplicationStatus status)
        {
            return await _context.Applications
                .Include(a => a.Student)
                .Include(a => a.DoctoralProgram)
                .Include(a => a.PrefferedMentor)
                .Where(a => a.ApplicationStatus == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Domain.Entities.Application>> GetByStudentIdAsync(int studentId)
        {
            return await _context.Applications
                .Include(a => a.Student)
                .Include(a => a.DoctoralProgram)
                .Include(a => a.PrefferedMentor)
                .Where(a => a.StudentId == studentId)
                .ToListAsync();
        }

        public async Task<bool> HasActiveApplicationAsync(int studentId, int programId)
        {
            return await _context.Applications
                .AnyAsync(a => a.StudentId == studentId &&
                               a.DoctoralProgramId == programId &&
                               (a.ApplicationStatus == ApplicationStatus.Draft ||
                                a.ApplicationStatus == ApplicationStatus.Submitted ||
                                a.ApplicationStatus == ApplicationStatus.UnderReview));
        }

        public async Task UpdateAsync(Domain.Entities.Application application)
        {
            _context.Applications.Update(application);
            await _context.SaveChangesAsync();
        }
    }
}
