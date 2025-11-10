using StriderWebApi.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace StriderWebApi.Domain.DomainClasses
{
    [Table("Microcycles")]
    public class Microcycle
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int WeekNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Sessions { get; set; } // Calculado automáticamente desde TrainingSessions
        public decimal Volume { get; set; } // en kilómetros - Calculado automáticamente desde TrainingSessions
        public MicrocycleIntensity Intensity { get; set; } = MicrocycleIntensity.Medium;
        public MicrocycleFocus? Focus { get; set; }

        // Relación con Mesocycle (REQUERIDA - un microciclo pertenece a un solo mesociclo)
        public int MesocycleId { get; set; }
        public Mesocycle Mesocycle { get; set; } = null!;

        // Relaciones con sesiones de entrenamiento
        public ICollection<TrainingSession> TrainingSessions { get; set; } = new List<TrainingSession>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
