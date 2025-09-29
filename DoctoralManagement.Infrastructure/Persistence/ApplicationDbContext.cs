using DoctoralManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DoctoralManagement.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Mentor> Mentors { get; set; }
        public DbSet<DoctoralProject> DoctoralProjects { get; set; }
        public DbSet<Publication> Publications { get; set; }
        public DbSet<Mobility> Mobilities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Fluent API configurations can be added here if needed
        }
    }
}
