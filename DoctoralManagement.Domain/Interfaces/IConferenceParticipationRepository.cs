using DoctoralManagement.Domain.Entities;

namespace DoctoralManagement.Domain.Interfaces
{
    public interface IConferenceParticipationRepository
    {
        Task<ConferenceParticipation> AddAsync(ConferenceParticipation participation);
        Task<ConferenceParticipation?> GetByIdAsync(int id);
        Task<IEnumerable<ConferenceParticipation>> GetByStudentIdAsync(int studentId);
        Task<IEnumerable<ConferenceParticipation>> GetAllAsync();
        Task UpdateAsync(ConferenceParticipation participation);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
