using System.ComponentModel.DataAnnotations.Schema;

namespace StriderWebApi.Domain.DomainClasses
{
    /// <summary>
    /// Agrupa intervalos dentro de una sesión o plantilla.
    /// </summary>
    [Table("TrainingSeries")]
    public class TrainingSeries
    {
        public int Id { get; set; }

        public int? TrainingSessionId { get; set; }
        public TrainingSession? TrainingSession { get; set; }

        public int? TrainingTemplateId { get; set; }
        public TrainingTemplate? TrainingTemplate { get; set; }

        public string Name { get; set; } = string.Empty;
        public int Repetitions { get; set; } = 1;
        public string RecoveryBetweenSets { get; set; } = "00:00";   // Formato MM:SS
        public int OrderIndex { get; set; } = 0;
        public string? Notes { get; set; }

        public ICollection<TrainingInterval> Intervals { get; set; } = new List<TrainingInterval>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
