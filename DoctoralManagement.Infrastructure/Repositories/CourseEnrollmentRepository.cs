using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using DoctoralManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DoctoralManagement.Infrastructure.Repositories
{
    public class CourseEnrollmentRepository : ICourseEnrollmentRepository
    {
        private readonly ApplicationDbContext _context;

        public CourseEnrollmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CourseEnrollment> AddAsync(CourseEnrollment enrollment)
        {
            _context.CourseEnrollments.Add(enrollment);
            await _context.SaveChangesAsync();
            return enrollment;
        }

        public async Task DeleteAsync(int id)
        {
            var enrollment = await _context.CourseEnrollments.FindAsync(id);
            if (enrollment != null)
            {
                _context.CourseEnrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.CourseEnrollments.AnyAsync(e  => e.Id == id);
        }

        public async Task<CourseEnrollment?> GetByIdAsync(int id)
        {
            return await _context.CourseEnrollments
                .Include(e => e.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(e  => e.Id == id);
        }

        public async Task<IEnumerable<CourseEnrollment>> GetByStudentIdAsync(int studentId)
        {
            return await _context.CourseEnrollments
                .Where(e => e.StudentId == studentId)
                .Include(e => e.Course)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<CourseEnrollment>> GetCompletedByStudentAsync(int studentId)
        {
            return await _context.CourseEnrollments
                .Where(e => e.StudentId == studentId && e.Completed)
                .Include(e => e.Course)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<CourseEnrollment>> GetCourseEnrollments()
        {
            return await _context.CourseEnrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<CourseEnrollment?> GetStudentCourseEnrollmentAsync(int studentId, int courseId)
        {
            return await _context.CourseEnrollments
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);
        }

        public async Task UpdateAsync(CourseEnrollment enrollment)
        {
            _context.CourseEnrollments.Update(enrollment);
            await _context.SaveChangesAsync();
        }
    }
}
