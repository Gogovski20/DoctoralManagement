using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DoctoralManagement.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        
        public DbSet<Student> Students { get; set; }
        public DbSet<Mentor> Mentors { get; set; }
        public DbSet<DoctoralProject> DoctoralProjects { get; set; }
        public DbSet<Publication> Publications { get; set; }
        public DbSet<Mobility> Mobilities { get; set; }
        public DbSet<DoctoralProgram> DoctoralPrograms { get; set; }
        public DbSet<ProgramMentor> ProgramMentors { get; set; }
        public DbSet<Domain.Entities.Application> Applications { get; set; }
        public DbSet<ECTSTracking> ECTSTrackings { get; set; }
        public DbSet<ThesisDefense> ThesisDefenses { get; set; }
        public DbSet<ConferenceParticipation> ConferenceParticipations { get; set; }
        public DbSet<CommitteeReview> CommitteeReviews { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseEnrollment> CourseEnrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureApplicationUser(modelBuilder);
            ConfigureStudent(modelBuilder);
            ConfigureMentor(modelBuilder);
            ConfigureDoctoralProgram(modelBuilder);
            ConfigureProgramMentor(modelBuilder);
            ConfigureApplication(modelBuilder);
            ConfigureECTSTracking(modelBuilder);
            ConfigureDoctoralProject(modelBuilder);
            ConfigurePublication(modelBuilder);
            ConfigureMobility(modelBuilder);
            ConfigureThesisDefense(modelBuilder);
            ConfigureConferenceParticipation(modelBuilder);
            ConfigureCommitteeReview(modelBuilder);
            ConfigureCourse(modelBuilder);
            ConfigureCourseEnrollment(modelBuilder);
        }

        private void ConfigureApplicationUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.FullName).HasMaxLength(200);

                entity.HasOne<Student>()
                    .WithMany()
                    .HasForeignKey(u => u.StudentId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne<Mentor>()
                    .WithMany()
                    .HasForeignKey(u => u.MentorId)
                    .OnDelete(DeleteBehavior.SetNull);



                entity.ToTable("AspNetUsers"); // keeps original identity table name
            });
        }


        private void ConfigureStudent(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.Property(s => s.Id)
                    .ValueGeneratedOnAdd();

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

                entity.ToTable("Students");
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

                entity.ToTable("Mentors");
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
                entity.Property(dp => dp.AvailableSlots).IsRequired();
                entity.Property(dp => dp.CurrentStudentsCount).IsRequired();

                entity.HasIndex(dp => dp.Name).IsUnique();

                entity.ToTable("DoctoralPrograms");
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

                entity.ToTable("ProgramMentors");
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

                entity.ToTable("Applications");
            });
        }

        private void ConfigureECTSTracking(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ECTSTracking>(entity =>
            {
                entity.HasKey(et => et.Id);

                entity.ToTable("ECTSTrackings");
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

                entity.ToTable("DoctoralProjects");
            });
        }

        private void ConfigurePublication(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Publication>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Title).IsRequired().HasMaxLength(500);
                entity.Property(p => p.Journal).IsRequired().HasMaxLength(200);

                entity.ToTable("Publications");
            });
        }

        private void ConfigureMobility(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Mobility>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Institution).IsRequired().HasMaxLength(200);
                entity.Property(m => m.Country).IsRequired().HasMaxLength(100);

                entity.ToTable("Mobilities");
            });
        }

        private void ConfigureThesisDefense(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ThesisDefense>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Room).HasMaxLength(100);

                entity.Property(d => d.CommitteeMemberIds)
                    .HasConversion(
                        v => string.Join(',', v),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList()
                    );

                entity.HasOne(d => d.DoctoralProject)
                    .WithMany(p => p.Defenses)
                    .HasForeignKey(d => d.DoctoralProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.ToTable("ThesisDefenses");
            });
        }

        private void ConfigureConferenceParticipation(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConferenceParticipation>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.ConferenceName).IsRequired().HasMaxLength(300);
                entity.HasOne(c => c.Student)
                      .WithMany() // later we can create navigation property
                      .HasForeignKey(c => c.StudentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.ToTable("ConferenceParticipations");
            });
        }

        private void ConfigureCommitteeReview(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CommitteeReview>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.Property(r => r.Comments).HasMaxLength(2000);

                entity.HasOne(r => r.ThesisDefense)
                    .WithMany(d => d.Reviews)  
                    .HasForeignKey(r => r.ThesisDefenseId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.ToTable("CommitteeReviews");
            });
        }

        private void ConfigureCourse(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnType("varchar(200)");

                entity.Property(e => e.EctsCredits)
                    .IsRequired();

                entity.Property(e => e.InstructorName)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnType("varchar(150)");

                entity.Property(e => e.Semester)
                    .IsRequired();

                entity.Property(e => e.Description)
                    .HasMaxLength(1000)
                    .HasColumnType("varchar(1000)");

                // Navigation
                entity.HasMany(e => e.Enrollments)
                    .WithOne(e => e.Course)
                    .HasForeignKey(e => e.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.ToTable("Courses");
            });
        }

        private void ConfigureCourseEnrollment(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<CourseEnrollment>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.StudentId).IsRequired();
                entity.Property(e => e.CourseId).IsRequired();
                entity.Property(e => e.EnrolledDate)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Completed)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.Grade)
                    .HasColumnType("numeric(3,1)");

                // Foreign keys
                entity.HasOne(e => e.Student)
                    .WithMany(s => s.CourseEnrollments)
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Course)
                    .WithMany(c => c.Enrollments)
                    .HasForeignKey(e => e.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.StudentId, e.CourseId })
                    .IsUnique();

                entity.ToTable("CourseEnrollments");
            });

        }

    }
}