using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using DoctoralManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DoctoralManagement.Infrastructure.Repositories
{
    public class DoctoralProjectRepository : IDoctoralProjectRepository
    {
        private readonly ApplicationDbContext _context;

        public DoctoralProjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DoctoralProject> AddAsync(DoctoralProject project)
        {
            _context.DoctoralProjects.Add(project);
            await _context.SaveChangesAsync();
            return project;
        }

        public async Task DeleteAsync(int id)
        {
            var project = await _context.DoctoralProjects.FindAsync(id);
            if (project != null)
            {
                _context.DoctoralProjects.Remove(project);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsActiveProjectForStudentAsync(int studentId)
        {
            return await _context.DoctoralProjects
                .AnyAsync(p => p.StudentId == studentId && 
                               (p.Status == ProjectStatus.Submitted || 
                                p.Status == ProjectStatus.UnderReview || 
                                p.Status == ProjectStatus.Approved));
        }

        public async Task<IEnumerable<DoctoralProject>> GetAllWithDetailsAsync()
        {
            return await _context.DoctoralProjects
                .Include(p => p.Student)
                .Include(p => p.Mentor)
                .ToListAsync();
        }

        public async Task<DoctoralProject?> GetByIdAsync(int id)
        {
            return await _context.DoctoralProjects
                .Include(p => p.Student)
                .Include(p => p.Mentor)
                .Include(p => p.Documents)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<DoctoralProject>> GetByMentorIdAsync(int mentorId)
        {
            return await _context.DoctoralProjects
                .Include(p => p.Student)
                .Include(p => p.Mentor)
                .Where(p => p.MentorId == mentorId)
                .ToListAsync();
        }

        public async Task<IEnumerable<DoctoralProject>> GetByStudentIdAsync(int studentId)
        {
            return await _context.DoctoralProjects
                .Include(p => p.Student)
                .Include(p => p.Mentor)
                .Where(p => p.StudentId == studentId)
                .ToListAsync();
        }

        public async Task UpdateAsync(DoctoralProject project)
        {
            _context.DoctoralProjects.Update(project);
            await _context.SaveChangesAsync();
        }
    }
}
