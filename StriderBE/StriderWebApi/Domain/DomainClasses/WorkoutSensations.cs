using System.ComponentModel.DataAnnotations.Schema;

namespace StriderWebApi.Domain.DomainClasses
{
    /// <summary>
    /// Sensaciones percibidas por el atleta durante el entrenamiento
    /// </summary>
    [Table("WorkoutSensations")]
    public class WorkoutSensations
    {
        public int Id { get; set; }

        // Sensaciones (escala 1-10)
        public int Effort { get; set; } // Esfuerzo percibido (1 = Muy fácil, 10 = Máximo esfuerzo)
        public int Fatigue { get; set; } // Fatiga (1 = Sin fatiga, 10 = Fatiga extrema)
        public int Motivation { get; set; } // Motivación (1 = Sin motivación, 10 = Muy motivado)
        public int MuscularLoad { get; set; } // Carga muscular (1 = Sin carga, 10 = Carga máxima)
        public int OverallFeeling { get; set; } // Sensación general (1 = Me sentí muy mal, 10 = Me sentí excelente)

        // Relación con CompletedWorkout (uno a uno)
        public int CompletedWorkoutId { get; set; }
        public CompletedWorkout CompletedWorkout { get; set; } = null!;
    }
}

