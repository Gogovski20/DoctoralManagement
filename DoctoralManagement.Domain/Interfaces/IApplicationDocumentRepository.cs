using DoctoralManagement.Domain.Entities;

namespace DoctoralManagement.Domain.Interfaces
{
    public interface IApplicationDocumentRepository
    {
        Task<ApplicationDocument?> GetByApplicationAndTypeAsync(int applicationId, ApplicationDocumentType type);
        Task<List<ApplicationDocument>> GetByApplicationIdAsync(int applicationId);
        Task<bool> HasAllRequiredDocumentsAsync(int applicationId);
        Task<ApplicationDocument?> GetByIdAsync(int documentId); 
        Task DeleteAsync(ApplicationDocument document);
    }
}
