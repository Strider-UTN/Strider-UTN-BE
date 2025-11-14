using System.ComponentModel.DataAnnotations.Schema;
using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Domain.DomainClasses
{
    /// <summary>
    /// Representa un entrenamiento completado por un atleta, relacionado con una sesión planificada
    /// </summary>
    [Table("CompletedWorkouts")]
    public class CompletedWorkout
    {
        public int Id { get; set; }

        // Campos básicos del entrenamiento (basados en APIWorkout)
        public string Name { get; set; } = string.Empty;
        public double Distance { get; set; } // km
        public DateTime Date { get; set; }
        public double Duration { get; set; } // segundos
        public double AverageHR { get; set; } // bpm
        public string? Comments { get; set; }

        // Relación obligatoria con TrainingSessionAthlete (incluye TrainingSession y Athlete)
        public int TrainingSessionAthleteId { get; set; }
        public TrainingSessionAthlete TrainingSessionAthlete { get; set; } = null!;

        // Fuente de carga (manual o Garmin)
        public WorkoutSource Source { get; set; } = WorkoutSource.Manual;

        // Calificación general del entrenamiento (asignada por el entrenador)
        public WorkoutRating? Rating { get; set; }

        // Metadatos
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navegación a entidades relacionadas
        public WorkoutSensations? Sensations { get; set; }
        public ICollection<WorkoutLap> Laps { get; set; } = new List<WorkoutLap>();
        public ICollection<WorkoutInjury> Injuries { get; set; } = new List<WorkoutInjury>();
        public WorkoutFeedback? Feedback { get; set; } // Feedback del entrenador (opcional - si existe, está revisado)
    }
}

