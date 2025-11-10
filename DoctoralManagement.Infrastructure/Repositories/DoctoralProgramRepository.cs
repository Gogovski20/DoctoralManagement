using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using DoctoralManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DoctoralManagement.Infrastructure.Repositories
{
    public class DoctoralProgramRepository : IDoctoralProgramRepository
    {
        private readonly ApplicationDbContext _context;

        public DoctoralProgramRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DoctoralProgram> AddAsync(DoctoralProgram doctoralProgram)
        {
            _context.DoctoralPrograms.Add(doctoralProgram);
            await _context.SaveChangesAsync();
            return doctoralProgram;
        }

        public async Task DeleteAsync(DoctoralProgram doctoralProgram)
        {
            _context.DoctoralPrograms.Remove(doctoralProgram);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.DoctoralPrograms.AnyAsync(p => p.Name == name);
        }

        public async Task<IEnumerable<DoctoralProgram>> GetAllAsync()
        {
            return await _context.DoctoralPrograms.ToListAsync();
        }

        public async Task<IEnumerable<DoctoralProgram>> GetAllNoTrackingAsync()
        {
            return await _context.DoctoralPrograms
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<DoctoralProgram> GetByIdAsync(int id)
        {
            return await _context.DoctoralPrograms.FindAsync(id);
        }

        public async Task<DoctoralProgram> GetByIdNoTrackingAsync(int id)
        {
            return await _context.DoctoralPrograms
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<DoctoralProgram> GetByNameAsync(string name)
        {
            return await _context.DoctoralPrograms
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Name == name);
        }

        public async Task<IEnumerable<DoctoralProgram>> GetByScientificAreaAsync(string scientificArea)
        {
            return await _context.DoctoralPrograms
                                 .Where(p => p.ScientificArea == scientificArea)
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task UpdateAsync(DoctoralProgram doctoralProgram)
        {
            var existing = await _context.DoctoralPrograms.FindAsync(doctoralProgram.Id);
            if (existing != null)
            {
                existing.Name = doctoralProgram.Name;
                existing.ScientificArea = doctoralProgram.ScientificArea;
                existing.Faculty = doctoralProgram.Faculty;
                existing.AvailableSlots = doctoralProgram.AvailableSlots;
                existing.CurrentStudentsCount = doctoralProgram.CurrentStudentsCount;
                existing.TuitionFee = doctoralProgram.TuitionFee;
                existing.InternationalTuitionFee = doctoralProgram.InternationalTuitionFee;
                existing.SpecialRequirements = doctoralProgram.SpecialRequirements;
                existing.IsActive = doctoralProgram.IsActive;

                await _context.SaveChangesAsync();
            }
        }
    }
}
