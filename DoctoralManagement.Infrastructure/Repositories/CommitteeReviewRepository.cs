using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using DoctoralManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DoctoralManagement.Infrastructure.Repositories
{
    public class CommitteeReviewRepository : ICommitteeReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public CommitteeReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommitteeReview> AddAsync(CommitteeReview review)
        {
            _context.CommitteeReviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<CommitteeReview?> GetByDefenseAndReviewerAsync(int defenseId, int reviewerId)
        {
            return await _context.CommitteeReviews
                .FirstOrDefaultAsync(r =>
                r.ThesisDefenseId == defenseId &&
                r.ReviewerId == reviewerId);
        }

        public async Task<List<CommitteeReview>> GetByDefenseIdAsync(int defenseId)
        {
            return await _context.CommitteeReviews
                .Where(r => r.ThesisDefenseId == defenseId)
                .ToListAsync();
        }

        public async Task UpdateAsync(CommitteeReview review)
        {
            _context.CommitteeReviews.Update(review);
            await _context.SaveChangesAsync();
        }
    }
}
