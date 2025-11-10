using StriderWebApi.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StriderWebApi.Domain.DomainClasses
{
    /// <summary>
    /// Plantilla de entrenamiento reutilizable
    /// </summary>
    [Table("TrainingTemplates")]
    public class TrainingTemplate
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TrainingType Type { get; set; }
        public TrainingCategory Category { get; set; }
        public int Duration { get; set; }
        public decimal? Distance { get; set; }
        public string? TargetPace { get; set; }
        public string? TargetHR { get; set; }
        public string Notes { get; set; } = string.Empty;

        [Range(1, 5)]
        public TrainingDifficulty Difficulty { get; set; }

        public bool IsFavorite { get; set; } = false;
        public int UseCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastUsed { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string[] Tags { get; set; } = Array.Empty<string>();

        // Warm-up / cool-down
        public int? WarmUpDuration { get; set; }
        public string? WarmUpPace { get; set; }
        public string? WarmUpDescription { get; set; }
        public int? CoolDownDuration { get; set; }
        public string? CoolDownPace { get; set; }
        public string? CoolDownDescription { get; set; }

        public int? CreatedByUserId { get; set; }

        public ICollection<TrainingSeries> Series { get; set; } = new List<TrainingSeries>();
        public ICollection<TrainingSession> Sessions { get; set; } = new List<TrainingSession>();
    }
}