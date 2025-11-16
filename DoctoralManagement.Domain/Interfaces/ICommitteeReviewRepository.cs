using DoctoralManagement.Domain.Entities;

namespace DoctoralManagement.Domain.Interfaces
{
    public interface ICommitteeReviewRepository
    {
        Task<CommitteeReview> AddAsync(CommitteeReview review);
        Task<List<CommitteeReview>> GetByDefenseIdAsync(int defenseId);
        Task<CommitteeReview?> GetByDefenseAndReviewerAsync(int defenseId, int reviewerId);
        Task UpdateAsync(CommitteeReview review);
    }
}
