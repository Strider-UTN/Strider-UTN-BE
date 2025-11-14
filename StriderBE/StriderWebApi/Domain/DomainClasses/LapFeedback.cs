using System.ComponentModel.DataAnnotations.Schema;

namespace StriderWebApi.Domain.DomainClasses
{
    /// <summary>
    /// Representa el feedback del entrenador sobre un lap específico de un entrenamiento
    /// </summary>
    [Table("LapFeedbacks")]
    public class LapFeedback
    {
        public int Id { get; set; }

        // Relación con WorkoutFeedback
        public int WorkoutFeedbackId { get; set; }
        public WorkoutFeedback WorkoutFeedback { get; set; } = null!;

        // Relación con WorkoutLap (el lap específico al que se refiere el feedback)
        public int WorkoutLapId { get; set; }
        public WorkoutLap WorkoutLap { get; set; } = null!;

        // Contenido del feedback para este lap
        public string Feedback { get; set; } = string.Empty;

        // Metadatos
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}

