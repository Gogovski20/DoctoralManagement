using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using DoctoralManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DoctoralManagement.Infrastructure.Repositories
{
    public class ThesisDefenseRepository : IThesisDefenseRepository
    {
        private readonly ApplicationDbContext _context;

        public ThesisDefenseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ThesisDefense> AddAsync(ThesisDefense defense)
        {
            _context.ThesisDefenses.Add(defense);
            await _context.SaveChangesAsync();
            return defense;
        }

        public async Task<bool> ExistsForProjectAsync(int projectId)
        {
            return await _context.ThesisDefenses
                .AnyAsync(d => d.DoctoralProjectId == projectId);
        }

        public async Task<IEnumerable<ThesisDefense>> GetAllAsync()
        {
            return await _context.ThesisDefenses
                .Include(t => t.DoctoralProject)
                    .ThenInclude(p => p.Student)
                .ToListAsync();
        }

        public async Task<ThesisDefense?> GetByIdAsync(int id)
        {
            return await _context.ThesisDefenses
                .Include(d => d.Reviews)
                .Include(d => d.DoctoralProject)
                    .ThenInclude(p => p.Student) 
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<ThesisDefense?> GetByProjectIdAsync(int projectId)
        {
            return await _context.ThesisDefenses
                .FirstOrDefaultAsync(d => d.DoctoralProjectId == projectId);
        }

        public async Task<IEnumerable<ThesisDefense>> GetByStatusAsync(DefenseStatus status)
        {
            return await _context.ThesisDefenses
                .Include(d => d.DoctoralProject)
                .ThenInclude(p => p.Student)
                .Where(d => d.Status == status)
                .OrderBy(d => d.ScheduledAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ThesisDefense>> GetStudentDefenses(int studentId)
        {
            return await _context.ThesisDefenses
                .Include(d => d.DoctoralProject)
                    .ThenInclude(p => p.Student)
                .Where(d => d.DoctoralProject.StudentId == studentId)
                .ToListAsync();
        }

        public async Task UpdateAsync(ThesisDefense defense)
        {
            _context.ThesisDefenses.Update(defense);
            await _context.SaveChangesAsync();
        }
    }
}
