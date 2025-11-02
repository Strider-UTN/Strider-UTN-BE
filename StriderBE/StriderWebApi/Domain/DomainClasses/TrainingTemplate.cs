using StriderWebApi.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StriderWebApi.Domain.DomainClasses
{
    /// <summary>
    /// Representa una plantilla de entrenamiento reutilizable
    /// </summary>
    [Table("TrainingTemplates")]
    public class TrainingTemplate
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public TrainingType Type { get; set; }

        public TrainingCategory Category { get; set; }

        public int Duration { get; set; } // en minutos

        public decimal? Distance { get; set; } // en kilómetros

        public string? TargetPace { get; set; } // Formato "MM:SS/km"

        public string? TargetHR { get; set; } // Ej: "85-90% FCMax"

        public string Notes { get; set; } = string.Empty;

        [Range(1, 5)] // Validación de dominio, se mantiene
        public TrainingDifficulty Difficulty { get; set; } // 1-5

        public bool IsFavorite { get; set; } = false;

        public int UseCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastUsed { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Tags como array JSON simple (JSONB en PostgreSQL)
        public string[] Tags { get; set; } = Array.Empty<string>();

        // Relación con intervalos (uno a muchos)
        public ICollection<TrainingInterval> Intervals { get; set; } = new List<TrainingInterval>();

        // Warm-up y Cool-down como propiedades separadas
        public int? WarmUpDuration { get; set; } // en minutos

        public string? WarmUpPace { get; set; }

        public string? WarmUpDescription { get; set; }

        public int? CoolDownDuration { get; set; } // en minutos

        public string? CoolDownPace { get; set; }

        public string? CoolDownDescription { get; set; }

        // FK para el usuario/entrenador que creó la plantilla
        public int? CreatedByUserId { get; set; }

        public ICollection<TrainingSession> Sessions { get; set; } = new List<TrainingSession>();
    }
}