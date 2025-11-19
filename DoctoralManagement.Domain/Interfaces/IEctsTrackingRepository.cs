using DoctoralManagement.Domain.Entities;

namespace DoctoralManagement.Domain.Interfaces
{
    public interface IEctsTrackingRepository
    {
        Task<ECTSTracking?> GetByStudentIdAsync(int studentId);
        Task<ECTSTracking?> GetByIdAsync(int id);
        Task<ECTSTracking> CreateAsync(ECTSTracking tracking);
        Task UpdateAsync(ECTSTracking tracking);
        Task DeleteAsync(int id);
        Task<IEnumerable<ECTSTracking>> GetAllAsync();
        Task<IEnumerable<ECTSTracking>> GetStudentsNearCompletionAsync(int threshold = 150);
    }
}
