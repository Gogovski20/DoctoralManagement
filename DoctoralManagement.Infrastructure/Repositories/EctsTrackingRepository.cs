using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using DoctoralManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DoctoralManagement.Infrastructure.Repositories
{
    public class EctsTrackingRepository : IEctsTrackingRepository
    {
        private readonly ApplicationDbContext _context;
        public EctsTrackingRepository(ApplicationDbContext context) => _context = context;

        public async Task<ECTSTracking?> GetByStudentIdAsync(int studentId) =>
            await _context.ECTSTrackings.FirstOrDefaultAsync(e => e.StudentId == studentId);

        public async Task<ECTSTracking?> GetByIdAsync(int id) =>
            await _context.ECTSTrackings.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);

        public async Task<ECTSTracking> CreateAsync(ECTSTracking tracking)
        {
            _context.ECTSTrackings.Add(tracking);
            await _context.SaveChangesAsync();
            return tracking;
        }

        public async Task UpdateAsync(ECTSTracking tracking)
        {
            _context.ECTSTrackings.Update(tracking);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var tracking = await _context.ECTSTrackings.FindAsync(id);
            if (tracking != null)
            {
                _context.ECTSTrackings.Remove(tracking);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ECTSTracking>> GetAllAsync() =>
            await _context.ECTSTrackings.AsNoTracking().ToListAsync();

        public async Task<IEnumerable<ECTSTracking>> GetStudentsNearCompletionAsync(int threshold = 150) =>
            await _context.ECTSTrackings
                .Where(e =>
                    e.OrganizedAcademicTraining +
                    e.IndependentResearchProject +
                    e.Publications +
                    e.TeachingActivities +
                    e.InternationalMobility >= threshold)
                .AsNoTracking()
                .ToListAsync();
    }
}
