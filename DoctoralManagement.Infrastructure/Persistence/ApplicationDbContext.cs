using DoctoralManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DoctoralManagement.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Existing DbSets
        public DbSet<Student> Students { get; set; }
        public DbSet<Mentor> Mentors { get; set; }
        public DbSet<DoctoralProject> DoctoralProjects { get; set; }
        public DbSet<Publication> Publications { get; set; }
        public DbSet<Mobility> Mobilities { get; set; }

        // NEW DbSets
        public DbSet<DoctoralProgram> DoctoralPrograms { get; set; }
        public DbSet<ProgramMentor> ProgramMentors { get; set; }
        public DbSet<Domain.Entities.Application> Applications { get; set; }
        public DbSet<ECTSTracking> ECTSTrackings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureStudent(modelBuilder);
            ConfigureMentor(modelBuilder);
            ConfigureDoctoralProgram(modelBuilder);
            ConfigureProgramMentor(modelBuilder);
            ConfigureApplication(modelBuilder);
            ConfigureECTSTracking(modelBuilder);
            ConfigureDoctoralProject(modelBuilder);
            ConfigurePublication(modelBuilder);
            ConfigureMobility(modelBuilder);
        }

        private void ConfigureStudent(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.FullName).IsRequired().HasMaxLength(200);
                entity.Property(s => s.Email).IsRequired().HasMaxLength(100);
                entity.Property(s => s.IndexNumber).IsRequired().HasMaxLength(20);
                entity.Property(s => s.EnglishCertificate).HasMaxLength(300);
                entity.Property(s => s.GPA).HasPrecision(4, 2); 

                entity.HasIndex(s => s.Email).IsUnique();
                entity.HasIndex(s => s.IndexNumber).IsUnique();

                // Relationships
                entity.HasOne(s => s.DoctoralProgram)
                      .WithMany(p => p.Students)
                      .HasForeignKey(s => s.DoctoralProgramId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.ECTSTracking)
                      .WithOne(et => et.Student)
                      .HasForeignKey<ECTSTracking>(et => et.StudentId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

        //private void ConfigureMentor(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Mentor>(entity =>
        //    {
        //        entity.HasKey(m => m.Id);
        //        entity.Property(m => m.FullName).IsRequired().HasMaxLength(200);
        //        entity.Property(m => m.Email).IsRequired().HasMaxLength(100);
        //        entity.Property(m => m.Department).IsRequired().HasMaxLength(100);
        //        entity.Property(m => m.Title).IsRequired().HasMaxLength(50);

        //        entity.HasIndex(m => m.Email).IsUnique();

        //        // Store ResearchAreas as JSON array in PostgreSQL
        //        entity.Property(m => m.ResearchAreas)
        //              .HasConversion(
        //                  v => string.Join(',', v),
        //                  v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
        //              );
        //    });
        //}

        private void ConfigureMentor(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Mentor>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.FullName).IsRequired().HasMaxLength(200);
                entity.Property(m => m.Email).IsRequired().HasMaxLength(100);
                entity.Property(m => m.Department).IsRequired().HasMaxLength(100);
                entity.Property(m => m.Title).IsRequired().HasMaxLength(50);

                entity.HasIndex(m => m.Email).IsUnique();

                // Store ResearchAreas as JSON array in PostgreSQL - WITH VALUE COMPARER
                entity.Property(m => m.ResearchAreas)
                      .HasConversion(
                          v => string.Join(',', v),
                          v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
                          new ValueComparer<List<string>>(
                              (c1, c2) => c1.SequenceEqual(c2),
                              c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                              c => c.ToList()
                          )
                      );
            });
        }

        private void ConfigureDoctoralProgram(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DoctoralProgram>(entity =>
            {
                entity.HasKey(dp => dp.Id);
                entity.Property(dp => dp.Name).IsRequired().HasMaxLength(300);
                entity.Property(dp => dp.ScientificArea).IsRequired().HasMaxLength(100);
                entity.Property(dp => dp.Faculty).IsRequired().HasMaxLength(200);
                entity.Property(dp => dp.SpecialRequirements).HasMaxLength(1000);
                entity.Property(dp => dp.TuitionFee).HasPrecision(10, 2);
                entity.Property(dp => dp.InternationalTuitionFee).HasPrecision(10, 2);

                entity.HasIndex(dp => dp.Name).IsUnique();
            });
        }

        private void ConfigureProgramMentor(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProgramMentor>(entity =>
            {
                // Composite key
                entity.HasKey(pm => new { pm.DoctoralProgramId, pm.MentorId });

                entity.Property(pm => pm.Role).IsRequired().HasMaxLength(50);

                // Relationships
                entity.HasOne(pm => pm.DoctoralProgram)
                      .WithMany(dp => dp.ProgramMentors)
                      .HasForeignKey(pm => pm.DoctoralProgramId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pm => pm.Mentor)
                      .WithMany(m => m.DoctoralPrograms)
                      .HasForeignKey(pm => pm.MentorId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void ConfigureApplication(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Domain.Entities.Application>(entity =>
            {
                entity.HasKey(a => a.Id);

                entity.Property(a => a.MotivationLetter).HasMaxLength(4000);
                entity.Property(a => a.ResearchProposal).HasMaxLength(4000);
                entity.Property(a => a.EnglishCertificatePath).HasMaxLength(500);

                // Relationships
                entity.HasOne(a => a.Student)
                      .WithMany(s => s.Applications)
                      .HasForeignKey(a => a.StudentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.DoctoralProgram)
                      .WithMany(p => p.Applications)
                      .HasForeignKey(a => a.DoctoralProgramId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.PrefferedMentor)
                      .WithMany()
                      .HasForeignKey(a => a.PrefferedMentorId)
                      .OnDelete(DeleteBehavior.SetNull);
            });
        }

        private void ConfigureECTSTracking(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ECTSTracking>(entity =>
            {
                entity.HasKey(et => et.Id);

                // One-to-one with Student is already configured in Student configuration
            });
        }

        private void ConfigureDoctoralProject(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DoctoralProject>(entity =>
            {
                entity.HasKey(dp => dp.Id);
                entity.Property(dp => dp.Title).IsRequired().HasMaxLength(500);
                entity.Property(dp => dp.ResearchArea).IsRequired().HasMaxLength(200);

                // Relationships
                entity.HasOne(dp => dp.Student)
                      .WithMany(s => s.DoctoralProjects)
                      .HasForeignKey(dp => dp.StudentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(dp => dp.Mentor)
                      .WithMany(m => m.DoctoralProjects)
                      .HasForeignKey(dp => dp.MentorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigurePublication(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Publication>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Title).IsRequired().HasMaxLength(500);
                entity.Property(p => p.Journal).IsRequired().HasMaxLength(200);
            });
        }

        private void ConfigureMobility(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Mobility>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Institution).IsRequired().HasMaxLength(200);
                entity.Property(m => m.Country).IsRequired().HasMaxLength(100);
            });
        }
    }
}