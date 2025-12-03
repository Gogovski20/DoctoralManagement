using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using DoctoralManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DoctoralManagement.Infrastructure.Repositories
{
    public class MobilityRepository : IMobilityRepository
    {
        private readonly ApplicationDbContext _context;
        public MobilityRepository(ApplicationDbContext context) => _context = context;

        public async Task<Mobility> AddAsync(Mobility mobility)
        {
            _context.Mobilities.Add(mobility);
            await _context.SaveChangesAsync();
            return mobility;
        }

        public async Task<Mobility?> GetByIdAsync(int id) =>
            await _context.Mobilities
                .Include(m => m.Document)
                .FirstOrDefaultAsync(m => m.Id == id);

        public async Task<IEnumerable<Mobility>> GetByStudentIdAsync(int studentId) =>
            await _context.Mobilities
                .Where(m => m.StudentId == studentId)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<Mobility>> GetAllAsync() =>
            await _context.Mobilities.AsNoTracking().ToListAsync();

        public async Task UpdateAsync(Mobility mobility)
        {
            _context.Mobilities.Update(mobility);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var mobility = await _context.Mobilities.FindAsync(id);
            if (mobility != null)
            {
                _context.Mobilities.Remove(mobility);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id) =>
            await _context.Mobilities.AnyAsync(m => m.Id == id);
    }
}
