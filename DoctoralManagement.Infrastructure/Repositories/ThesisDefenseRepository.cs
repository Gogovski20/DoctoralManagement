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

        public async Task<ThesisDefense?> GetByProjectIdAsync(int projectId)
        {
            return await _context.ThesisDefenses
                .FirstOrDefaultAsync(d => d.DoctoralProjectId == projectId);
        }
    }
}
