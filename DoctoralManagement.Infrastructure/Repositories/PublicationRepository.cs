using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using DoctoralManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DoctoralManagement.Infrastructure.Repositories
{
    public class PublicationRepository : IPublicationRepository
    {
        private readonly ApplicationDbContext _context;

        public PublicationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Publication> AddAsync(Publication publication)
        {
            _context.Publications.Add(publication);
            await _context.SaveChangesAsync();
            return publication;
        }

        public async Task DeleteAsync(int id)
        {
            var publication = await _context.Publications.FindAsync(id);
            if (publication != null)
            {
                _context.Publications.Remove(publication);
                await _context.SaveChangesAsync();
            }
        }

        public Task<bool> ExistsAsync(int id)
        {
            return _context.Publications.AnyAsync(p  => p.Id == id);
        }

        public async Task<IEnumerable<Publication>> GetAllAsync()
        {
            return await _context.Publications
                .Include(p => p.Student)
                .Include(p => p.Document)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Publication?> GetByIdAsync(int id)
        {
            return await _context.Publications
                .Include(p => p.Student)
                .Include(p => p.Document)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Publication>> GetByStudentIdAsync(int studentId)
        {
            return await _context.Publications
                .Include(p => p.Document)
                .Where(p  => p.StudentId == studentId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> GetPublicationCountByStudentAsync(int studentId)
        {
            return await _context.Publications
                .CountAsync(p  => p.StudentId == studentId);
        }

        public async Task UpdateAsync(Publication publication)
        {
            _context.Publications.Update(publication);
            await _context.SaveChangesAsync();
        }
    }
}
