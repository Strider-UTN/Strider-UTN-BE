using System.ComponentModel.DataAnnotations.Schema;
using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Domain.DomainClasses
{
    /// <summary>
    /// Representa una molestia o dolor reportado durante un entrenamiento específico
    /// Diferente de AthleteInjury que es una lesión general del atleta
    /// </summary>
    [Table("WorkoutInjuries")]
    public class WorkoutInjury
    {
        public int Id { get; set; }

        public InjuryLocation BodyPart { get; set; } // Zona corporal afectada
        public int Severity { get; set; } // Escala 1-10
        public string Description { get; set; } = string.Empty;
        public bool AffectedPerformance { get; set; } // Si afectó el rendimiento
        public InjuryType Type { get; set; } // Molestia o Dolor

        // Relación con CompletedWorkout
        public int CompletedWorkoutId { get; set; }
        public CompletedWorkout CompletedWorkout { get; set; } = null!;
    }
}

