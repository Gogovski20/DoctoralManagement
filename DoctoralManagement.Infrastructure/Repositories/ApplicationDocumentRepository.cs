using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using DoctoralManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DoctoralManagement.Infrastructure.Repositories
{
    public class ApplicationDocumentRepository : IApplicationDocumentRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationDocumentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task DeleteAsync(ApplicationDocument document)
        {
            _context.ApplicationDocuments.Remove(document);
            await _context.SaveChangesAsync();
        }

        public async Task<ApplicationDocument?> GetByApplicationAndTypeAsync(int applicationId, ApplicationDocumentType type)
        {
            return await _context.ApplicationDocuments
                .FirstOrDefaultAsync(ad => ad.ApplicationId == applicationId && ad.DocumentType == type);
        }

        public async Task<List<ApplicationDocument>> GetByApplicationIdAsync(int applicationId)
        {
            return await _context.ApplicationDocuments
                .Where(ad => ad.ApplicationId == applicationId)
                .OrderBy(ad => ad.DocumentType)
                .ToListAsync();
        }

        public async Task<ApplicationDocument?> GetByIdAsync(int documentId)
        {
            return await _context.ApplicationDocuments
                .FindAsync(documentId);
        }

        public async Task<bool> HasAllRequiredDocumentsAsync(int applicationId)
        {
            var requiredTypes = new[]
            {
                ApplicationDocumentType.MotivationLetter,
                ApplicationDocumentType.ResearchProposal,
                ApplicationDocumentType.EnglishCertificate
            };

            var uploadedDocs = await _context.ApplicationDocuments
                .Where(ad => ad.ApplicationId == applicationId)
                .Select(ad => ad.DocumentType)
                .ToListAsync();

            return requiredTypes.All(rt => uploadedDocs.Contains(rt));
        }
    }
}
