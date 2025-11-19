using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using DoctoralManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DoctoralManagement.Infrastructure.Repositories
{
    public class ConferenceParticipationRepository : IConferenceParticipationRepository
    {
        private readonly ApplicationDbContext _context;
        public ConferenceParticipationRepository(ApplicationDbContext context) => _context = context;

        public async Task<ConferenceParticipation> AddAsync(ConferenceParticipation participation)
        {
            _context.ConferenceParticipations.Add(participation);
            await _context.SaveChangesAsync();
            return participation;
        }

        public async Task<ConferenceParticipation?> GetByIdAsync(int id) =>
            await _context.ConferenceParticipations.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

        public async Task<IEnumerable<ConferenceParticipation>> GetByStudentIdAsync(int studentId) =>
            await _context.ConferenceParticipations
                .Where(c => c.StudentId == studentId)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<ConferenceParticipation>> GetAllAsync() =>
            await _context.ConferenceParticipations.AsNoTracking().ToListAsync();

        public async Task UpdateAsync(ConferenceParticipation participation)
        {
            _context.ConferenceParticipations.Update(participation);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var participation = await _context.ConferenceParticipations.FindAsync(id);
            if (participation != null)
            {
                _context.ConferenceParticipations.Remove(participation);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id) =>
            await _context.ConferenceParticipations.AnyAsync(c => c.Id == id);
    }
}
