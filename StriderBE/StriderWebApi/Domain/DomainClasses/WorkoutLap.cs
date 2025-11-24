using System.ComponentModel.DataAnnotations.Schema;

namespace StriderWebApi.Domain.DomainClasses
{
    /// <summary>
    /// Representa una vuelta/intervalo de un entrenamiento completado
    /// Basado en APILap del backend
    /// </summary>
    [Table("WorkoutLaps")]
    public class WorkoutLap
    {
        public int Id { get; set; }

        public int Index { get; set; }
        public double Distance { get; set; } // meters
        public double Duration { get; set; } // segundos
        public double AverageHR { get; set; } // bpm
        public double Speed { get; set; } // m/s (calculado de distance/duration)
        public DateTime StartTime { get; set; }

        // Relación con CompletedWorkout
        public int CompletedWorkoutId { get; set; }
        public CompletedWorkout CompletedWorkout { get; set; } = null!;

        // Navegación a feedback del entrenador (opcional)
        public LapFeedback? LapFeedback { get; set; }
    }
}

