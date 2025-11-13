using System.ComponentModel.DataAnnotations.Schema;

namespace StriderWebApi.Domain.DomainClasses
{
    /// <summary>
    /// Representa el feedback del entrenador sobre un entrenamiento completado
    /// </summary>
    [Table("WorkoutFeedbacks")]
    public class WorkoutFeedback
    {
        public int Id { get; set; }

        // Relación obligatoria con CompletedWorkout (uno-a-uno)
        public int CompletedWorkoutId { get; set; }
        public CompletedWorkout CompletedWorkout { get; set; } = null!;

        // Relación con el entrenador que proporciona el feedback
        public int CoachId { get; set; }
        public Coach Coach { get; set; } = null!;

        // Contenido del feedback
        public string Feedback { get; set; } = string.Empty; // Feedback general del entrenamiento
        public string? Recommendations { get; set; } // Recomendaciones del entrenador

        // Feedback por lap (opcional, para entrenamientos con intervalos)
        public ICollection<LapFeedback> LapFeedbacks { get; set; } = new List<LapFeedback>();

        // Metadatos
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}

