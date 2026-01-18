using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BeFit.Models;
using Microsoft.AspNetCore.Identity;

namespace BeFit.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<ExerciseType> ExerciseTypes => Set<ExerciseType>();
        public DbSet<TrainingSession> TrainingSessions => Set<TrainingSession>();
        public DbSet<TrainingEntry> TrainingEntries => Set<TrainingEntry>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ExerciseType>()
                   .HasIndex(e => e.Name)
                   .IsUnique();

            builder.Entity<TrainingSession>()
                   .HasOne(ts => ts.User)
                   .WithMany()
                   .HasForeignKey(ts => ts.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TrainingEntry>()
                   .HasOne(te => te.TrainingSession)
                   .WithMany()
                   .HasForeignKey(te => te.TrainingSessionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TrainingEntry>()
                   .HasOne(te => te.ExerciseType)
                   .WithMany()
                   .HasForeignKey(te => te.ExerciseTypeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TrainingEntry>()
                   .HasOne(te => te.User)
                   .WithMany()
                   .HasForeignKey(te => te.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Map Weight property to WeightKg column (legacy database schema)
            builder.Entity<TrainingEntry>()
                   .Property(te => te.Weight)
                   .HasColumnName("WeightKg");

            // Map Repetitions property to Reps column (legacy database schema)
            builder.Entity<TrainingEntry>()
                   .Property(te => te.Repetitions)
                   .HasColumnName("Reps");
        }
    }
}
