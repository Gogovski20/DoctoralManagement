using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using DoctoralManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DoctoralManagement.Infrastructure.Repositories
{
    public class MentorRepository : IMentorRepository
    {
        private readonly ApplicationDbContext _context;

        public MentorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Mentor> AddAsync(Mentor mentor)
        {
            _context.Mentors.Add(mentor);
            await _context.SaveChangesAsync();
            return mentor;
        }

        public async Task DeleteAsync(int id)
        {
            var mentor = await _context.Mentors.FindAsync(id);
            if (mentor != null)
            {
                _context.Mentors.Remove(mentor);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Mentor>> GetAllAsync()
        {
            return await _context.Mentors
                .Where(m => m.IsActive)
                .ToListAsync();
        }

        public async Task<Mentor?> GetByIdAsync(int id)
        {
            return await _context.Mentors
                .Include(m => m.DoctoralProjects)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<bool> IsAvailableForNewStudentAsync(int mentorId)
        {
            var mentor = await _context.Mentors
                .Include(m => m.DoctoralProjects)
                .FirstOrDefaultAsync(m => m.Id == mentorId);

            if (mentor == null)
            {
                throw new Exception($"Mentor with id {mentorId} not found");
            }

            var activeProjectsCount = mentor.DoctoralProjects
                .Count(p => p.Status != ProjectStatus.Rejected && p.Status != ProjectStatus.Completed);

            return activeProjectsCount < mentor.MaxStudents;
        }

        public async Task UpdateAsync(Mentor mentor)
        {
            _context.Mentors.Update(mentor);
            await _context.SaveChangesAsync();
        }
    }
}
