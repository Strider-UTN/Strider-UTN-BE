using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Dto.CompletedWorkout
{
    public class CreateCompletedWorkoutDto
    {
        public int TrainingSessionAthleteId { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Distance { get; set; } // meters
        public DateTime Date { get; set; }
        public double Duration { get; set; }
        public double AverageHR { get; set; } // bpm
        public string? Comments { get; set; }
        
        // Sensaciones (opcional pero recomendado)
        public CreateWorkoutSensationsDto? Sensations { get; set; }
        
        // Laps (opcional)
        public List<CreateWorkoutLapDto>? Laps { get; set; }
        
        // Molestias/Dolores (opcional)
        public List<CreateWorkoutInjuryDto>? Injuries { get; set; }
    }

    public class CreateWorkoutSensationsDto
    {
        public int Effort { get; set; } // 1-10
        public int Fatigue { get; set; } // 1-10
        public int Motivation { get; set; } // 1-10
        public int MuscularLoad { get; set; } // 1-10
        public int OverallFeeling { get; set; } // 1-10
    }

    public class CreateWorkoutLapDto
    {
        public int Index { get; set; }
        public double Distance { get; set; } // meters
        public double Duration { get; set; } // segundos
        public double AverageHR { get; set; } // bpm
        public DateTime StartTime { get; set; }
        // Speed se calculará automáticamente en el servicio
    }

    public class CreateWorkoutInjuryDto
    {
        public InjuryLocation BodyPart { get; set; }
        public int Severity { get; set; } // 1-10
        public string Description { get; set; } = string.Empty;
        public bool AffectedPerformance { get; set; }
        public InjuryType Type { get; set; }
    }

    public class CompletedWorkoutResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Distance { get; set; } // meters
        public DateTime Date { get; set; }
        public double Duration { get; set; } // segundos (se puede convertir a mm:ss en el frontend)
        public double AverageHR { get; set; } // bpm
        public string? Comments { get; set; }
        
        // Relaciones
        public int TrainingSessionAthleteId { get; set; }
        public int TrainingSessionId { get; set; }
        public string TrainingSessionName { get; set; } = string.Empty;
        public int AthleteId { get; set; }
        public string AthleteName { get; set; } = string.Empty;
        
        // Información de planificación
        public int? PlanningId { get; set; }
        public string? PlanningName { get; set; }
        public int? MesocycleId { get; set; }
        public string? MesocycleName { get; set; }
        public int? MicrocycleId { get; set; }
        public string? MicrocycleName { get; set; }
        
        // Categoría de la sesión
        public TrainingCategory? Category { get; set; }
        
        // Entidades relacionadas
        public WorkoutSensationsResponseDto? Sensations { get; set; }
        public List<WorkoutLapResponseDto> Laps { get; set; } = new();
        public List<WorkoutInjuryResponseDto> Injuries { get; set; } = new();
        
        // Feedback del entrenador (si existe, está revisado)
        public WorkoutFeedbackResponseDto? Feedback { get; set; }
        public WorkoutRating? Rating { get; set; } // Calificación general
        
        // Metadatos
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class WorkoutSensationsResponseDto
    {
        public int Id { get; set; }
        public int Effort { get; set; }
        public int Fatigue { get; set; }
        public int Motivation { get; set; }
        public int MuscularLoad { get; set; }
        public int OverallFeeling { get; set; }
    }

    public class WorkoutLapResponseDto
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public double Distance { get; set; } // meters
        public double Duration { get; set; } // segundos
        public double AverageHR { get; set; } // bpm
        public double Speed { get; set; } // m/s
        public DateTime StartTime { get; set; }
    }

    public class WorkoutInjuryResponseDto
    {
        public int Id { get; set; }
        public InjuryLocation BodyPart { get; set; }
        public int Severity { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool AffectedPerformance { get; set; }
        public InjuryType Type { get; set; }
    }

    public class WorkoutFeedbackResponseDto
    {
        public int Id { get; set; }
        public int CoachId { get; set; }
        public string CoachName { get; set; } = string.Empty;
        public string Feedback { get; set; } = string.Empty;
        public string? Recommendations { get; set; }
        public WorkoutRating Rating { get; set; }
        public List<LapFeedbackResponseDto> LapFeedbacks { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class LapFeedbackResponseDto
    {
        public int Id { get; set; }
        public int WorkoutLapId { get; set; }
        public string Feedback { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    // DTO para agrupar workouts por atleta (para la vista de retroalimentación del coach)
    public class CompletedWorkoutsGroupedByAthleteDto
    {
        public int AthleteId { get; set; }
        public string AthleteName { get; set; } = string.Empty;
        public int? TrainingGroupId { get; set; }
        public string? TrainingGroupName { get; set; }
        public List<CompletedWorkoutResponseDto> Workouts { get; set; } = new();
    }

    // DTOs para crear/actualizar feedback del entrenador
    public class CreateWorkoutFeedbackDto
    {
        public WorkoutRating Rating { get; set; }
        public string Feedback { get; set; } = string.Empty;
        public string? Recommendations { get; set; }
        public List<CreateLapFeedbackDto>? LapFeedbacks { get; set; }
    }

    public class CreateLapFeedbackDto
    {
        public int WorkoutLapId { get; set; }
        public string Feedback { get; set; } = string.Empty;
    }

    public class UpdateWorkoutFeedbackDto
    {
        public WorkoutRating? Rating { get; set; }
        public string? Feedback { get; set; }
        public string? Recommendations { get; set; }
        public List<CreateLapFeedbackDto>? LapFeedbacks { get; set; }
    }
}

