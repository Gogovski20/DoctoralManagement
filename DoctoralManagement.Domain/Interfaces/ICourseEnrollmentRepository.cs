using DoctoralManagement.Domain.Entities;

namespace DoctoralManagement.Domain.Interfaces
{
    public interface ICourseEnrollmentRepository
    {
        Task<CourseEnrollment> AddAsync(CourseEnrollment enrollment);
        Task<CourseEnrollment?> GetByIdAsync(int id);
        Task<IEnumerable<CourseEnrollment>> GetByStudentIdAsync(int studentId);
        Task<IEnumerable<CourseEnrollment>> GetCompletedByStudentAsync(int studentId);
        Task<CourseEnrollment?> GetStudentCourseEnrollmentAsync(int studentId, int courseId);
        Task UpdateAsync(CourseEnrollment enrollment);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
