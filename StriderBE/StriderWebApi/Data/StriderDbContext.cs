using Microsoft.EntityFrameworkCore;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Data
{
    public class StriderDbContext(DbContextOptions<StriderDbContext> options) : DbContext(options)
    {

        #region Constructors
        #endregion

        #region DbSets
        public DbSet<User> Users => Set<User>();
        public DbSet<Coach> Coaches => Set<Coach>();
        public DbSet<Athlete> Athletes => Set<Athlete>();
        public DbSet<Session> Sessions => Set<Session>();
        public DbSet<Workout> Workouts => Set<Workout>();
        public DbSet<Interval> Intervals => Set<Interval>();
        public DbSet<Lap> Laps => Set<Lap>();
        public DbSet<Team> Teams => Set<Team>();
        public DbSet<TrainingLocation> TrainingLocations => Set<TrainingLocation>();
        public DbSet<TrainingPlan> TrainingPlans => Set<TrainingPlan>();
        public DbSet<Ailment> Ailments => Set<Ailment>();
        #endregion

        #region Overrides
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasDiscriminator<UserTypeEnum>("UserType")
                .HasValue<Coach>(UserTypeEnum.Coach)
                .HasValue<Athlete>(UserTypeEnum.Athlete);

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Session>()
                .Property(s => s.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Workout>()
                .Property(w => w.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Interval>()
                .Property(i => i.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Lap>()
                .Property(l => l.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Team>()
                .Property(t => t.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<TrainingLocation>()
                .Property(tl => tl.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<TrainingPlan>()
                .Property(tp => tp.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Ailment>()
                .Property(a => a.Id)
                .ValueGeneratedOnAdd();
        }
        #endregion
    }
}
