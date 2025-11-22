using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using StriderWebApi.Domain;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Domain.Enums;
using System.Text.Json;

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
        public DbSet<Workout> Workouts => Set<Workout>();

        public DbSet<Lap> Laps => Set<Lap>();
        public DbSet<Team> Teams => Set<Team>();
        public DbSet<TrainingLocation> TrainingLocations => Set<TrainingLocation>();
        public DbSet<TrainingPlan> TrainingPlans => Set<TrainingPlan>();
        public DbSet<AthleteInjury> AthleteInjuries => Set<AthleteInjury>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<TrainingTemplate> TrainingTemplates => Set<TrainingTemplate>();
        public DbSet<TrainingSeries> TrainingSeries => Set<TrainingSeries>();
        public DbSet<TrainingInterval> TrainingIntervals => Set<TrainingInterval>();
        public DbSet<TrainingSession> TrainingSessions => Set<TrainingSession>();
        public DbSet<TrainingSessionAthlete> TrainingSessionAthletes => Set<TrainingSessionAthlete>();
        public DbSet<CoachAthleteRelationship> CoachAthleteRelationships => Set<CoachAthleteRelationship>();
        public DbSet<TrainingGroup> TrainingGroups => Set<TrainingGroup>();   
        public DbSet<TrainingPoint> TrainingPoints => Set<TrainingPoint>();
        public DbSet<TrainingGroupMember> TrainingGroupMembers => Set<TrainingGroupMember>();
        public DbSet<Planning> Plannings => Set<Planning>();
        public DbSet<Period> Periods => Set<Period>(); 
        public DbSet<Mesocycle> Mesocycles => Set<Mesocycle>();
        public DbSet<Microcycle> Microcycles => Set<Microcycle>();
        public DbSet<PlanningAthlete> PlanningAthletes => Set<PlanningAthlete>();
        public DbSet<CompletedWorkout> CompletedWorkouts => Set<CompletedWorkout>();
        public DbSet<WorkoutSensations> WorkoutSensations => Set<WorkoutSensations>();
        public DbSet<WorkoutLap> WorkoutLaps => Set<WorkoutLap>();
        public DbSet<WorkoutInjury> WorkoutInjuries => Set<WorkoutInjury>();
        public DbSet<WorkoutFeedback> WorkoutFeedbacks => Set<WorkoutFeedback>();
        public DbSet<LapFeedback> LapFeedbacks => Set<LapFeedback>();
        public DbSet<VO2MaxSuggestion> VO2MaxSuggestions => Set<VO2MaxSuggestion>();
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
                .HasIndex(u => new { u.Email, u.UserType })
                .IsUnique();

            modelBuilder.Entity<Athlete>()
                .Property(a => a.RestingHeartRate)
                .HasDefaultValue(60.0)
                .IsRequired();

            modelBuilder.Entity<Athlete>()
                .Property(a => a.MaximumHeartRate)
                .HasDefaultValue(200.0)
                .IsRequired();

            modelBuilder.Entity<Athlete>()
                .Property(a => a.ThresholdHeartRate)
                .HasDefaultValue(150.0)
                .IsRequired();

            modelBuilder.Entity<Workout>()
                .Property(w => w.Id)
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

            modelBuilder.Entity<AthleteInjury>(entity =>
            {
                entity.ToTable("AthleteInjuries");
                entity.HasKey(i => i.Id);
                entity.Property(i => i.Id).ValueGeneratedOnAdd();

                entity.Property(i => i.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(i => i.Description)
                    .HasMaxLength(2000);

                entity.Property(i => i.Notes)
                    .HasMaxLength(2000);

                entity.Property(i => i.Severity)
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.Property(i => i.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.Property(i => i.AffectedArea)
                    .HasConversion<string>()
                    .HasMaxLength(30);

                entity.Property(i => i.DiagnosisDate)
                    .IsRequired();

                entity.Property(i => i.RecoveryEstimateDate);
                entity.Property(i => i.RecoveryDate);

                entity.Property(i => i.Treatment)
                    .HasConversion<string>()
                    .HasMaxLength(50);

                entity.Property(i => i.ImpactOnTraining)
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.Property(i => i.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAdd();

                entity.Property(i => i.UpdatedAt);

                entity.HasOne(i => i.Athlete)
                    .WithMany(a => a.Injuries)
                    .HasForeignKey(i => i.AthleteId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(i => i.AthleteId);
                entity.HasIndex(i => new { i.AthleteId, i.Status });
                entity.HasIndex(i => i.DiagnosisDate);
            });

            modelBuilder.Entity<Notification>()
                .Property(n => n.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Notification>()
                .Property(n => n.Type)
                .HasConversion<string>();

            // Configuración de TrainingTemplate
            modelBuilder.Entity<TrainingTemplate>(entity =>
            {
                entity.ToTable("TrainingTemplates");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Type).IsRequired().HasConversion<string>().HasMaxLength(20);
                entity.Property(e => e.Category).IsRequired().HasConversion<string>().HasMaxLength(30);
                entity.Property(e => e.Duration).IsRequired();
                entity.Property(e => e.TargetPace).HasMaxLength(10);
                entity.Property(e => e.TargetHR).HasMaxLength(50);
                entity.Property(e => e.Notes).HasMaxLength(2000);
                entity.Property(e => e.Difficulty).IsRequired().HasConversion<int>();
                entity.Property(e => e.IsFavorite).IsRequired().HasDefaultValue(false);
                entity.Property(e => e.UseCount).IsRequired().HasDefaultValue(0);
                entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Tags)
                      .HasColumnType("jsonb")
                      .HasConversion(
                          v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                          v => JsonSerializer.Deserialize<string[]>(v, (JsonSerializerOptions?)null) ?? Array.Empty<string>())
                      .Metadata.SetValueComparer(
                          new ValueComparer<string[]>(
                              (a, b) => a!.SequenceEqual(b!),
                              a => a.Aggregate(0, (hash, v) => HashCode.Combine(hash, v.GetHashCode())),
                              a => a.ToArray()));

                entity.Property(e => e.WarmUpPace).HasMaxLength(10);
                entity.Property(e => e.WarmUpDescription).HasMaxLength(500);
                entity.Property(e => e.CoolDownPace).HasMaxLength(10);
                entity.Property(e => e.CoolDownDescription).HasMaxLength(500);

                entity.HasMany(e => e.Series)
                      .WithOne(s => s.TrainingTemplate)
                      .HasForeignKey(s => s.TrainingTemplateId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.IsFavorite);
                entity.HasIndex(e => e.CreatedByUserId);
                entity.HasIndex(e => new { e.Type, e.Category });
                entity.HasIndex(e => e.Tags).HasMethod("gin");
            });

            // Configuración de TrainingInterval
            modelBuilder.Entity<TrainingInterval>(entity =>
            {
                entity.ToTable("TrainingIntervals");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Type).IsRequired().HasConversion<string>().HasMaxLength(20);
                entity.Property(e => e.Repetitions).IsRequired();
                entity.Property(e => e.Distance).IsRequired();
                entity.Property(e => e.TargetTime).HasMaxLength(10);
                entity.Property(e => e.RecoveryTime).IsRequired().HasMaxLength(10).HasDefaultValue("00:00");
                entity.Property(e => e.PaceType).IsRequired().HasConversion<string>();
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Intensity).HasConversion<string>();
                entity.Property(e => e.TrainingMode).HasConversion<string>();
                entity.Property(e => e.Duration).HasMaxLength(10);
                entity.Property(e => e.TargetSpeed).HasMaxLength(10);
                entity.Property(e => e.OrderIndex).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.TrainingSeries)
                      .WithMany(s => s.Intervals)
                      .HasForeignKey(e => e.TrainingSeriesId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.TrainingSeriesId);
                entity.HasIndex(e => new { e.TrainingSeriesId, e.OrderIndex });
            });

            // Configuración de TrainingSession
            modelBuilder.Entity<TrainingSession>(entity =>
            {
                entity.ToTable("TrainingSessions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.Category).IsRequired().HasConversion<string>().HasMaxLength(30);
                entity.Property(e => e.Notes).HasMaxLength(2000);
                entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Microcycle)
                      .WithMany(e => e.TrainingSessions)
                      .HasForeignKey(e => e.MicrocycleId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Planning)
                      .WithMany(e => e.TrainingSessions)
                      .HasForeignKey(e => e.PlanningId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.CreatedBy)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Template)
                      .WithMany(t => t.Sessions)
                      .HasForeignKey(e => e.TemplateId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(e => e.Series)
                      .WithOne(s => s.TrainingSession)
                      .HasForeignKey(s => s.TrainingSessionId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.PlanningId);
                entity.HasIndex(e => e.MicrocycleId);
                entity.HasIndex(e => e.Date);
                entity.HasIndex(e => e.CreatedByUserId);
                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.TemplateId);
            });

            // Configuración de TrainingSessionAthlete
            modelBuilder.Entity<TrainingSessionAthlete>(entity =>
            {
                entity.ToTable("TrainingSessionAthletes");

                entity.HasKey(e => e.Id);

                // Configurar Id como auto-increment
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.TrainingSessionId)
                    .IsRequired();

                entity.Property(e => e.AthleteId)
                    .IsRequired();

                // Relación con TrainingSession
                entity.HasOne(e => e.TrainingSession)
                    .WithMany(s => s.Athletes)
                    .HasForeignKey(e => e.TrainingSessionId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con Athlete/User
                entity.HasOne(e => e.Athlete)
                    .WithMany()
                    .HasForeignKey(e => e.AthleteId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.Property(e => e.CompletedAt);

                // Datos reales del entrenamiento
                entity.Property(e => e.ActualDistance);
                entity.Property(e => e.ActualDuration);
                entity.Property(e => e.ActualAvgPace)
                    .HasMaxLength(10);
                entity.Property(e => e.ActualAvgHR);
                entity.Property(e => e.ActualMaxHR);

                // Metadata
                entity.Property(e => e.AssignedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Índice compuesto único para evitar duplicados
                entity.HasIndex(e => new { e.TrainingSessionId, e.AthleteId })
                    .IsUnique();

                // Otros índices
                entity.HasIndex(e => e.AthleteId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.CompletedAt); // ✅ Sin filtro
                entity.HasIndex(e => new { e.AthleteId, e.Status });
                entity.HasIndex(e => new { e.TrainingSessionId, e.Status });
            });

            // Configuración de CompletedWorkout
            modelBuilder.Entity<CompletedWorkout>(entity =>
            {
                entity.ToTable("CompletedWorkouts");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Distance)
                    .IsRequired();

                entity.Property(e => e.Date)
                    .IsRequired();

                entity.Property(e => e.Duration)
                    .IsRequired(); // segundos

                entity.Property(e => e.AverageHR)
                    .IsRequired();

                entity.Property(e => e.Comments)
                    .HasMaxLength(2000);


                entity.Property(e => e.Rating)
                    .HasConversion<string>()
                    .HasMaxLength(30);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Relación obligatoria con TrainingSessionAthlete
                entity.HasOne(e => e.TrainingSessionAthlete)
                    .WithMany(tsa => tsa.CompletedWorkouts)
                    .HasForeignKey(e => e.TrainingSessionAthleteId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación uno-a-uno con WorkoutSensations
                entity.HasOne(e => e.Sensations)
                    .WithOne(s => s.CompletedWorkout)
                    .HasForeignKey<WorkoutSensations>(s => s.CompletedWorkoutId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación uno-a-muchos con WorkoutLaps
                entity.HasMany(e => e.Laps)
                    .WithOne(l => l.CompletedWorkout)
                    .HasForeignKey(l => l.CompletedWorkoutId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación uno-a-muchos con WorkoutInjuries
                entity.HasMany(e => e.Injuries)
                    .WithOne(i => i.CompletedWorkout)
                    .HasForeignKey(i => i.CompletedWorkoutId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Índices
                entity.HasIndex(e => e.TrainingSessionAthleteId);
                entity.HasIndex(e => e.Date);
                entity.HasIndex(e => new { e.TrainingSessionAthleteId, e.Date });
            });

            // Configuración de WorkoutSensations
            modelBuilder.Entity<WorkoutSensations>(entity =>
            {
                entity.ToTable("WorkoutSensations");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Effort)
                    .IsRequired();

                entity.Property(e => e.Fatigue)
                    .IsRequired();

                entity.Property(e => e.Motivation)
                    .IsRequired();

                entity.Property(e => e.MuscularLoad)
                    .IsRequired();

                entity.Property(e => e.OverallFeeling)
                    .IsRequired();

                // Check constraints a nivel de entidad
                entity.HasCheckConstraint("CK_WorkoutSensations_Effort", "\"Effort\" >= 1 AND \"Effort\" <= 10");
                entity.HasCheckConstraint("CK_WorkoutSensations_Fatigue", "\"Fatigue\" >= 1 AND \"Fatigue\" <= 10");
                entity.HasCheckConstraint("CK_WorkoutSensations_Motivation", "\"Motivation\" >= 1 AND \"Motivation\" <= 10");
                entity.HasCheckConstraint("CK_WorkoutSensations_MuscularLoad", "\"MuscularLoad\" >= 1 AND \"MuscularLoad\" <= 10");
                entity.HasCheckConstraint("CK_WorkoutSensations_OverallFeeling", "\"OverallFeeling\" >= 1 AND \"OverallFeeling\" <= 10");

                // Relación con CompletedWorkout (uno-a-uno)
                entity.HasOne(e => e.CompletedWorkout)
                    .WithOne(w => w.Sensations)
                    .HasForeignKey<WorkoutSensations>(e => e.CompletedWorkoutId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Índice único para asegurar uno-a-uno
                entity.HasIndex(e => e.CompletedWorkoutId)
                    .IsUnique();
            });

            // Configuración de WorkoutLap
            modelBuilder.Entity<WorkoutLap>(entity =>
            {
                entity.ToTable("WorkoutLaps");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Index)
                    .IsRequired();

                entity.Property(e => e.Distance)
                    .IsRequired();

                entity.Property(e => e.Duration)
                    .IsRequired(); // segundos

                entity.Property(e => e.AverageHR)
                    .IsRequired();

                entity.Property(e => e.Speed)
                    .IsRequired();

                entity.Property(e => e.StartTime)
                    .IsRequired();

                // Relación con CompletedWorkout
                entity.HasOne(e => e.CompletedWorkout)
                    .WithMany(w => w.Laps)
                    .HasForeignKey(e => e.CompletedWorkoutId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Índices
                entity.HasIndex(e => e.CompletedWorkoutId);
                entity.HasIndex(e => new { e.CompletedWorkoutId, e.Index });
            });

            // Configuración de WorkoutInjury
            modelBuilder.Entity<WorkoutInjury>(entity =>
            {
                entity.ToTable("WorkoutInjuries");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.BodyPart)
                    .IsRequired()
                    .HasConversion<string>()
                    .HasMaxLength(30);

                entity.Property(e => e.Severity)
                    .IsRequired();

                // Check constraint a nivel de entidad
                entity.HasCheckConstraint("CK_WorkoutInjury_Severity", "\"Severity\" >= 1 AND \"Severity\" <= 10");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.AffectedPerformance)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasConversion<string>()
                    .HasMaxLength(20);

                // Relación con CompletedWorkout
                entity.HasOne(e => e.CompletedWorkout)
                    .WithMany(w => w.Injuries)
                    .HasForeignKey(e => e.CompletedWorkoutId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Índices
                entity.HasIndex(e => e.CompletedWorkoutId);
            });

            // Configuración de WorkoutFeedback
            modelBuilder.Entity<WorkoutFeedback>(entity =>
            {
                entity.ToTable("WorkoutFeedbacks");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Feedback)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.Recommendations)
                    .HasMaxLength(2000);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Relación uno-a-uno con CompletedWorkout
                entity.HasOne(e => e.CompletedWorkout)
                    .WithOne(w => w.Feedback)
                    .HasForeignKey<WorkoutFeedback>(f => f.CompletedWorkoutId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con Coach
                entity.HasOne(e => e.Coach)
                    .WithMany()
                    .HasForeignKey(e => e.CoachId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación uno-a-muchos con LapFeedbacks
                entity.HasMany(e => e.LapFeedbacks)
                    .WithOne(lf => lf.WorkoutFeedback)
                    .HasForeignKey(lf => lf.WorkoutFeedbackId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Índices
                entity.HasIndex(e => e.CompletedWorkoutId)
                    .IsUnique(); // Uno-a-uno con CompletedWorkout
                entity.HasIndex(e => e.CoachId);
                entity.HasIndex(e => e.CreatedAt);
            });

            // Configuración de LapFeedback
            modelBuilder.Entity<LapFeedback>(entity =>
            {
                entity.ToTable("LapFeedbacks");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Feedback)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Relación con WorkoutFeedback
                entity.HasOne(e => e.WorkoutFeedback)
                    .WithMany(wf => wf.LapFeedbacks)
                    .HasForeignKey(e => e.WorkoutFeedbackId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación uno-a-uno con WorkoutLap
                entity.HasOne(e => e.WorkoutLap)
                    .WithOne(wl => wl.LapFeedback)
                    .HasForeignKey<LapFeedback>(lf => lf.WorkoutLapId)
                    .OnDelete(DeleteBehavior.Restrict); // No eliminar el lap si se elimina el feedback

                // Índices
                entity.HasIndex(e => e.WorkoutFeedbackId);
                entity.HasIndex(e => e.WorkoutLapId)
                    .IsUnique(); // Un lap solo puede tener un feedback
            });

            // Configuración de CoachAthleteRelationship
            modelBuilder.Entity<CoachAthleteRelationship>(entity =>
            {
                entity.ToTable("CoachAthleteRelationships");

                entity.HasKey(e => e.Id);

                // Configurar Id como auto-increment
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                // Relación con Coach (User)
                entity.HasOne(e => e.Coach)
                    .WithMany()
                    .HasForeignKey(e => e.CoachId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con Athlete (User)
                entity.HasOne(e => e.Athlete)
                    .WithMany()
                    .HasForeignKey(e => e.AthleteId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Configuración de propiedades
                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.Property(e => e.InvitationMessage)
                    .HasMaxLength(1000);

                entity.Property(e => e.InvitedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.RespondedAt);

                entity.Property(e => e.LinkedSince);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UpdatedAt);

                // Índice único compuesto para evitar duplicados (mismo coach con mismo atleta)
                entity.HasIndex(e => new { e.CoachId, e.AthleteId })
                    .IsUnique();

                // Índices para búsquedas frecuentes
                entity.HasIndex(e => e.CoachId);
                entity.HasIndex(e => e.AthleteId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => new { e.AthleteId, e.Status });
                entity.HasIndex(e => new { e.CoachId, e.Status });
            });

            modelBuilder.Entity<TrainingGroup>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .HasMaxLength(1000);

                entity.Property(e => e.CreatedDate)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Relación con User (CreatedBy)
                entity.HasOne(e => e.CreatedBy)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con TrainingPoints (uno-a-muchos)
                entity.HasMany(e => e.TrainingPoints)
                    .WithOne(e => e.TrainingGroup)
                    .HasForeignKey(e => e.TrainingGroupId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con TrainingGroupMembers (uno-a-muchos)
                entity.HasMany(e => e.Members)
                    .WithOne(e => e.TrainingGroup)
                    .HasForeignKey(e => e.TrainingGroupId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Owned Entity para Notifications
                entity.OwnsOne(e => e.Notifications);

                // Índices
                entity.HasIndex(e => e.CreatedByUserId);
                entity.HasIndex(e => e.Name);
            });

            // TrainingPoint
            modelBuilder.Entity<TrainingPoint>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Relación con TrainingGroup
                entity.HasOne(e => e.TrainingGroup)
                    .WithMany(e => e.TrainingPoints)
                    .HasForeignKey(e => e.TrainingGroupId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Índices
                entity.HasIndex(e => e.TrainingGroupId);
            });

            // TrainingGroupMember
            modelBuilder.Entity<TrainingGroupMember>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Status como enum (conversión a int en base de datos)
                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasConversion<int>()
                    .HasDefaultValue(TrainingGroupMemberStatus.Pending);

                entity.Property(e => e.JoinedDate)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.InvitationMessage)
                    .HasMaxLength(500);

                // Relación con TrainingGroup
                entity.HasOne(e => e.TrainingGroup)
                    .WithMany(e => e.Members)
                    .HasForeignKey(e => e.TrainingGroupId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con User (siempre será un atleta)
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Índice único para evitar miembros duplicados en la misma sede
                entity.HasIndex(e => new { e.TrainingGroupId, e.UserId })
                    .IsUnique();

                // Índices adicionales
                entity.HasIndex(e => e.TrainingGroupId);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Status);
            });

            modelBuilder.Entity<Planning>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .HasMaxLength(1000);

                entity.Property(e => e.StartDate)
                    .IsRequired();

                entity.Property(e => e.EndDate);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasConversion<int>()
                    .HasDefaultValue(PlanningStatus.Draft);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UpdatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Relación con Coach
                entity.HasOne(e => e.Coach)
                    .WithMany()
                    .HasForeignKey(e => e.CoachId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con Periods (opcional - para agrupación)
                entity.HasMany(e => e.Periods)
                    .WithOne(e => e.Planning)
                    .HasForeignKey(e => e.PlanningId)
                    .OnDelete(DeleteBehavior.SetNull); // SetNull porque es opcional

                // Relación con Mesocycles (directa - estructura principal)
                entity.HasMany(e => e.Mesocycles)
                    .WithOne(e => e.Planning)
                    .HasForeignKey(e => e.PlanningId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con TrainingSessions (para optimización de consultas directas)
                entity.HasMany(e => e.TrainingSessions)
                    .WithOne(e => e.Planning)
                    .HasForeignKey(e => e.PlanningId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Índices
                entity.HasIndex(e => e.CoachId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.StartDate);
            });

            modelBuilder.Entity<Period>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.StartWeek)
                    .IsRequired();

                entity.Property(e => e.EndWeek)
                    .IsRequired();

                entity.Property(e => e.Objective)
                    .HasMaxLength(500);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasConversion<int>()
                    .HasDefaultValue(PeriodStatus.Planning);

                entity.Property(e => e.PlanningId);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UpdatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Relación opcional con Planning (para contexto)
                entity.HasOne(e => e.Planning)
                    .WithMany(e => e.Periods)
                    .HasForeignKey(e => e.PlanningId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Relación con Mesocycles (one-to-many - un mesociclo pertenece a un solo período, opcional)
                entity.HasMany(e => e.Mesocycles)
                    .WithOne(e => e.Period)
                    .HasForeignKey(e => e.PeriodId)
                    .OnDelete(DeleteBehavior.SetNull); // SetNull porque es opcional

                // Índices
                entity.HasIndex(e => e.PlanningId);
                entity.HasIndex(e => e.Status);
            });

            modelBuilder.Entity<Mesocycle>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.StartDate)
                    .IsRequired();

                entity.Property(e => e.EndDate)
                    .IsRequired();

                entity.Property(e => e.Objective)
                    .HasMaxLength(500);

                entity.Property(e => e.WeeksCount)
                    .IsRequired();

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasConversion<int>()
                    .HasDefaultValue(MesocycleStatus.Planning);

                entity.Property(e => e.PlanningId)
                    .IsRequired();

                entity.Property(e => e.PeriodId);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UpdatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Relación directa con Planning (REQUERIDA - estructura principal)
                entity.HasOne(e => e.Planning)
                    .WithMany(e => e.Mesocycles)
                    .HasForeignKey(e => e.PlanningId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación opcional con Period (para agrupación - un mesociclo pertenece a un solo período)
                entity.HasOne(e => e.Period)
                    .WithMany(e => e.Mesocycles)
                    .HasForeignKey(e => e.PeriodId)
                    .OnDelete(DeleteBehavior.SetNull); // SetNull porque es opcional

                // Relación con Microcycles (un microciclo pertenece a un solo mesociclo)
                entity.HasMany(e => e.Microcycles)
                    .WithOne(e => e.Mesocycle)
                    .HasForeignKey(e => e.MesocycleId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Índices
                entity.HasIndex(e => e.PlanningId);
                entity.HasIndex(e => e.PeriodId); // NUEVO
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.StartDate);
                entity.HasIndex(e => e.EndDate);
            });

            modelBuilder.Entity<Microcycle>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .HasMaxLength(1000); // Opcional, sin IsRequired()

                entity.Property(e => e.WeekNumber)
                    .IsRequired();

                entity.Property(e => e.StartDate)
                    .IsRequired();

                entity.Property(e => e.EndDate)
                    .IsRequired();

                entity.Property(e => e.Sessions)
                    .IsRequired();

                entity.Property(e => e.Volume)
                    .IsRequired()
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.Intensity)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasConversion<string>();

                entity.Property(e => e.Focus)
                    .HasConversion<int>();

                entity.Property(e => e.MesocycleId)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UpdatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Relación con Mesocycle (REQUERIDA - un microciclo pertenece a un solo mesociclo)
                entity.HasOne(e => e.Mesocycle)
                    .WithMany(e => e.Microcycles)
                    .HasForeignKey(e => e.MesocycleId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con TrainingSessions (REQUERIDA - todas las sesiones están dentro de un microciclo)
                entity.HasMany(e => e.TrainingSessions)
                    .WithOne(e => e.Microcycle)
                    .HasForeignKey(e => e.MicrocycleId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Índices
                entity.HasIndex(e => e.MesocycleId);
                entity.HasIndex(e => e.StartDate);
                entity.HasIndex(e => e.EndDate);
            });

            modelBuilder.Entity<PlanningAthlete>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.PlanningId)
                    .IsRequired();

                entity.Property(e => e.AthleteId)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Relación con Planning
                entity.HasOne(e => e.Planning)
                    .WithMany(e => e.PlanningAthletes)
                    .HasForeignKey(e => e.PlanningId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con Athlete
                entity.HasOne(e => e.Athlete)
                    .WithMany()
                    .HasForeignKey(e => e.AthleteId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Índice único para evitar duplicados
                entity.HasIndex(e => new { e.PlanningId, e.AthleteId })
                    .IsUnique();

                // Índices adicionales
                entity.HasIndex(e => e.PlanningId);
                entity.HasIndex(e => e.AthleteId);
            });

            modelBuilder.Entity<TrainingSeries>(entity =>
            {
                entity.ToTable("TrainingSeries");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Repetitions).IsRequired().HasDefaultValue(1);
                entity.Property(e => e.RecoveryBetweenSets).IsRequired().HasMaxLength(10).HasDefaultValue("00:00");
                entity.Property(e => e.OrderIndex).IsRequired();
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.TrainingSession)
                      .WithMany(s => s.Series)
                      .HasForeignKey(e => e.TrainingSessionId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .IsRequired(false);

                entity.HasOne(e => e.TrainingTemplate)
                      .WithMany(t => t.Series)
                      .HasForeignKey(e => e.TrainingTemplateId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .IsRequired(false);

                entity.HasMany(e => e.Intervals)
                      .WithOne(i => i.TrainingSeries)
                      .HasForeignKey(i => i.TrainingSeriesId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .IsRequired();

                entity.HasIndex(e => e.TrainingSessionId);
                entity.HasIndex(e => e.TrainingTemplateId);
                entity.HasIndex(e => new { e.TrainingSessionId, e.OrderIndex });
                entity.HasIndex(e => new { e.TrainingTemplateId, e.OrderIndex });

                entity.ToTable(tb => tb.HasCheckConstraint(
                    "CK_TrainingSeries_SingleParent",
                    "((\"TrainingSessionId\" IS NULL AND \"TrainingTemplateId\" IS NOT NULL) OR " +
                    "(\"TrainingSessionId\" IS NOT NULL AND \"TrainingTemplateId\" IS NULL))"));
            });
        }
        #endregion
    }
}
