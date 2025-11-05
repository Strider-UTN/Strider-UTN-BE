using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using StriderWebApi.Domain;
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
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<TrainingTemplate> TrainingTemplates => Set<TrainingTemplate>();
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

            modelBuilder.Entity<User>()
                .HasIndex(u => new { u.Username, u.UserType })
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
                .HasDiscriminator<string>("AilmentType")
                .HasValue<Injury>(nameof(Injury))
                .HasValue<Illness>(nameof(Illness));

            modelBuilder.Entity<Ailment>()
                .HasOne(a => a.Athlete)
                .WithMany(a => a.Ailments)
                .HasForeignKey(a => a.AthleteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ailment>()
                .Property(a => a.Severity)
                .HasConversion<string>();

            modelBuilder.Entity<Injury>()
                .Property(i => i.Type)
                .HasConversion<string>();

            modelBuilder.Entity<Injury>()
                .Property(i => i.Location)
                .HasConversion<string>();

            modelBuilder.Entity<Illness>()
                .Property(i => i.Type)
                .HasConversion<string>();

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

                // Configurar Id como auto-increment
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                // Configuración de columnas con nombres PascalCase
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .HasMaxLength(1000);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.Property(e => e.Category)
                    .IsRequired()
                    .HasConversion<string>()
                    .HasMaxLength(30);

                entity.Property(e => e.Duration)
                    .IsRequired();

                entity.Property(e => e.TargetPace)
                    .HasMaxLength(10);

                entity.Property(e => e.TargetHR)
                    .HasMaxLength(50);

                entity.Property(e => e.Notes)
                    .HasMaxLength(2000);

                entity.Property(e => e.Difficulty)
                    .IsRequired()
                    .HasConversion<int>();

                entity.Property(e => e.IsFavorite)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.UseCount)
                    .IsRequired()
                    .HasDefaultValue(0);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Configurar tags como JSONB (PostgreSQL)
                entity.Property(e => e.Tags)
                    .HasColumnType("jsonb")
                    .HasConversion(
                        v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                        v => System.Text.Json.JsonSerializer.Deserialize<string[]>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? Array.Empty<string>()
                    )
                    .Metadata.SetValueComparer(
                        new ValueComparer<string[]>(
                            (c1, c2) => c1!.SequenceEqual(c2!),
                            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                            c => c.ToArray()
                        )
                    );

                // Warm-up y Cool-down
                entity.Property(e => e.WarmUpPace)
                    .HasMaxLength(10);

                entity.Property(e => e.WarmUpDescription)
                    .HasMaxLength(500);

                entity.Property(e => e.CoolDownPace)
                    .HasMaxLength(10);

                entity.Property(e => e.CoolDownDescription)
                    .HasMaxLength(500);

                // Índices para mejorar búsquedas
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.IsFavorite);
                entity.HasIndex(e => e.CreatedByUserId);
                entity.HasIndex(e => new { e.Type, e.Category });

                // Índice GIN para búsqueda eficiente en tags (JSONB)
                entity.HasIndex(e => e.Tags)
                      .HasMethod("gin");

                // Relación uno a muchos con intervalos (eliminación en cascada)
                entity.HasMany(e => e.Intervals)
                    .WithOne(e => e.TrainingTemplate)
                    .HasForeignKey(e => e.TrainingTemplateId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired(false); // Opcional porque ahora puede pertenecer a Session

                // Relación uno a muchos con sesiones (opcional, solo si agregaste la relación inversa)
                entity.HasMany(e => e.Sessions)
                    .WithOne(s => s.Template)
                    .HasForeignKey(s => s.TemplateId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configuración de TrainingInterval
            modelBuilder.Entity<TrainingInterval>(entity =>
            {
                entity.ToTable("TrainingIntervals");

                entity.HasKey(e => e.Id);

                // Configurar Id como auto-increment
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                // Relación opcional con Template
                entity.HasOne(e => e.TrainingTemplate)
                    .WithMany(t => t.Intervals)
                    .HasForeignKey(e => e.TrainingTemplateId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired(false);

                // Relación opcional con Session
                entity.HasOne(e => e.TrainingSession)
                    .WithMany(s => s.Intervals)
                    .HasForeignKey(e => e.TrainingSessionId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired(false);

                // Check constraint (usando ToTable con HasCheckConstraint)
                entity.ToTable(tb => tb.HasCheckConstraint(
                    "CK_TrainingInterval_SingleParent",
                    "((\"TrainingTemplateId\" IS NULL AND \"TrainingSessionId\" IS NOT NULL) OR " +
                    "(\"TrainingTemplateId\" IS NOT NULL AND \"TrainingSessionId\" IS NULL))"
                ));

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.Property(e => e.Repetitions)
                    .IsRequired();

                entity.Property(e => e.Distance)
                    .IsRequired();

                entity.Property(e => e.TargetTime)
                    .HasMaxLength(10);

                entity.Property(e => e.RecoveryTime)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasDefaultValue("00:00");

                entity.Property(e => e.PaceType)
                    .IsRequired()
                    .HasConversion<string>();

                entity.Property(e => e.Description)
                    .HasMaxLength(500);

                entity.Property(e => e.Intensity)
                    .HasConversion<string>();

                entity.Property(e => e.TrainingMode)
                    .HasConversion<string>();

                entity.Property(e => e.Duration)
                    .HasMaxLength(10);

                entity.Property(e => e.TargetSpeed)
                    .HasMaxLength(10);

                entity.Property(e => e.OrderIndex)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => e.TrainingTemplateId);
                entity.HasIndex(e => e.TrainingSessionId);
                entity.HasIndex(e => new { e.TrainingTemplateId, e.OrderIndex });
                entity.HasIndex(e => new { e.TrainingSessionId, e.OrderIndex });
            });

            // Configuración de TrainingSession
            modelBuilder.Entity<TrainingSession>(entity =>
            {
                entity.ToTable("TrainingSessions");

                entity.HasKey(e => e.Id);

                // Configurar Id como auto-increment
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .HasMaxLength(1000);

                entity.Property(e => e.Date)
                    .IsRequired();

                entity.Property(e => e.Category)
                    .IsRequired()
                    .HasConversion<string>()
                    .HasMaxLength(30);

                entity.Property(e => e.Notes)
                    .HasMaxLength(2000);

                // Relación con Microcycle (REQUERIDA - un entrenamiento pertenece a un solo microciclo)
                entity.HasOne(e => e.Microcycle)
                    .WithMany(e => e.TrainingSessions)
                    .HasForeignKey(e => e.MicrocycleId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con Planning (REQUERIDA - para optimización de consultas y validación de integridad)
                entity.HasOne(e => e.Planning)
                    .WithMany(e => e.TrainingSessions)
                    .HasForeignKey(e => e.PlanningId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con el usuario que crea
                entity.HasOne(e => e.CreatedBy)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación opcional con Template
                entity.HasOne(e => e.Template)
                    .WithMany(t => t.Sessions)
                    .HasForeignKey(e => e.TemplateId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Metadata
                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UpdatedAt);

                // Índices para mejorar búsquedas
                entity.HasIndex(e => e.PlanningId); // NUEVO
                entity.HasIndex(e => e.MicrocycleId); // NUEVO
                entity.HasIndex(e => e.Date);
                entity.HasIndex(e => e.CreatedByUserId);
                entity.HasIndex(e => e.TemplateId);
                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => new { e.CreatedByUserId, e.Date });
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

                // Relación con Microcycles (one-to-many - un microciclo debe pertenecer a un período)
                entity.HasMany(e => e.Microcycles)
                    .WithOne(e => e.Period)
                    .HasForeignKey(e => e.PeriodId)
                    .OnDelete(DeleteBehavior.Restrict); // Restrict porque el período es requerido para microciclos

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

                entity.Property(e => e.PeriodId)
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

                // Relación con Period (REQUERIDA - un microciclo debe pertenecer a un período)
                entity.HasOne(e => e.Period)
                    .WithMany(e => e.Microcycles)
                    .HasForeignKey(e => e.PeriodId)
                    .OnDelete(DeleteBehavior.Restrict); // Restrict porque el período es requerido

                // Relación con TrainingSessions (REQUERIDA - todas las sesiones están dentro de un microciclo)
                entity.HasMany(e => e.TrainingSessions)
                    .WithOne(e => e.Microcycle)
                    .HasForeignKey(e => e.MicrocycleId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Índices
                entity.HasIndex(e => e.MesocycleId);
                entity.HasIndex(e => e.PeriodId); // NUEVO
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
        }
        #endregion
    }
}
