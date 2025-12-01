using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using DoctoralManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DoctoralManagement.Infrastructure.Repositories
{
    public class ActivityDocumentRepository : IActivityDocumentRepository
    {
        private readonly ApplicationDbContext _context;

        public ActivityDocumentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ActivityDocument> AddAsync(ActivityDocument document)
        {
            _context.ActivityDocuments.Add(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task DeleteAsync(ActivityDocument document)
        {
            _context.ActivityDocuments.Remove(document);
            await _context.SaveChangesAsync();
        }

        public async Task<ActivityDocument?> GetByConferenceIdAsync(int conferenceId)
        {
            return await _context.ActivityDocuments
                .FirstOrDefaultAsync(d => d.ConferenceId == conferenceId);
        }

        public async Task<ActivityDocument?> GetByDoctoralProjectIdAsync(int projectId)
        {
            return await _context.ActivityDocuments
                .FirstOrDefaultAsync(d => d.DoctoralProjectId == projectId);
        }

        public async Task<ActivityDocument> GetByIdAsync(int id)
        {
            return await _context.ActivityDocuments
                .FirstAsync(d => d.Id == id);
        }

        public async Task<ActivityDocument?> GetByMobilityIdAsync(int mobilityId)
        {
            return await _context.ActivityDocuments
                .FirstOrDefaultAsync(d => d.MobilityId == mobilityId);
        }

        public async Task<ActivityDocument?> GetByPublicationIdAsync(int publicationId)
        {
            return await _context.ActivityDocuments
                .FirstOrDefaultAsync(d => d.PublicationId == publicationId);
        }

        public async Task<List<ActivityDocument>> GetPendingReviewAsync(ActivityDocumentType type)
        {
            return await _context.ActivityDocuments
                .Where(d => d.DocumentType == type && d.Status == DocumentStatus.Pending)
                .ToListAsync();
        }

        public async Task UpdateAsync(ActivityDocument document)
        {
            _context.ActivityDocuments.Update(document);
            await _context.SaveChangesAsync();
        }
    }
}
