using DoctoralManagement.Domain.Entities;

namespace DoctoralManagement.Domain.Interfaces
{
    public interface IActivityDocumentRepository
    {
        Task<ActivityDocument> GetByIdAsync(int id);
        Task<ActivityDocument?> GetByPublicationIdAsync(int publicationId);
        Task<ActivityDocument?> GetByMobilityIdAsync(int mobilityId);
        Task<ActivityDocument?> GetByConferenceIdAsync(int conferenceId);
        Task<ActivityDocument?> GetByDoctoralProjectIdAsync(int projectId);
        Task<List<ActivityDocument>> GetPendingReviewAsync(ActivityDocumentType type);
        Task UpdateAsync(ActivityDocument document);
        Task DeleteAsync(ActivityDocument document);
        Task<ActivityDocument> AddAsync(ActivityDocument document);
    }
}
