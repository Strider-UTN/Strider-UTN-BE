using StriderWebApi.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace StriderWebApi.Domain.DomainClasses
{
    /// <summary>
    /// Representa un intervalo dentro de una plantilla de entrenamiento o una sesión de entrenamiento
    /// </summary>
    [Table("TrainingIntervals")]
    public class TrainingInterval
    {
        public int Id { get; set; }

        public int? TrainingTemplateId { get; set; }

        public int? TrainingSessionId { get; set; }

        public TrainingIntervalType Type { get; set; }

        public int Repetitions { get; set; }

        public decimal Distance { get; set; } // en metros

        public string? TargetTime { get; set; } // Formato "MM:SS" o "HH:MM:SS"

        public string RecoveryTime { get; set; } = "00:00"; // Formato "MM:SS"

        public PaceType PaceType { get; set; }

        public decimal? Pace { get; set; } // en minutos por kilómetro

        public decimal? Vo2MaxPercentage { get; set; }

        public string? Description { get; set; }

        public IntensityLevel? Intensity { get; set; }

        public TrainingMode? TrainingMode { get; set; }

        public string? Duration { get; set; } // Formato "MM:SS" o "HH:MM:SS"

        public string? TargetSpeed { get; set; } // Formato "MM:SS/km"

        public int OrderIndex { get; set; } // Para mantener el orden de los intervalos

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navegación (modificar para que sea opcional)
        public TrainingTemplate? TrainingTemplate { get; set; } // CAMBIO: Ahora nullable

        // Navegación nueva
        public TrainingSession? TrainingSession { get; set; } // NUEVO
    }
}
