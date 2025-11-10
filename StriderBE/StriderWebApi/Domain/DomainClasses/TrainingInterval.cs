using StriderWebApi.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace StriderWebApi.Domain.DomainClasses
{
    /// <summary>
    /// Intervalo atómico dentro de una serie
    /// </summary>
    [Table("TrainingIntervals")]
    public class TrainingInterval
    {
        public int Id { get; set; }

        public int TrainingSeriesId { get; set; }
        public TrainingSeries TrainingSeries { get; set; } = null!;

        public TrainingIntervalType Type { get; set; }
        public int Repetitions { get; set; }
        public decimal Distance { get; set; }                    // metros
        public string? TargetTime { get; set; }                  // MM:SS o HH:MM:SS
        public string RecoveryTime { get; set; } = "00:00";      // MM:SS
        public PaceType PaceType { get; set; }
        public decimal? Pace { get; set; }                       // min/km
        public decimal? Vo2MaxPercentage { get; set; }
        public string? Description { get; set; }
        public IntensityLevel? Intensity { get; set; }
        public TrainingMode? TrainingMode { get; set; }
        public string? Duration { get; set; }                    // MM:SS o HH:MM:SS
        public string? TargetSpeed { get; set; }                 // MM:SS/km
        public int OrderIndex { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
