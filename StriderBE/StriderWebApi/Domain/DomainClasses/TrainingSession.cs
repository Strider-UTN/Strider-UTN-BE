using StriderWebApi.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace StriderWebApi.Domain.DomainClasses
{
    /// <summary>
    /// Representa una sesión de entrenamiento programada para uno o más atletas
    /// </summary>
    [Table("TrainingSessions")]
    public class TrainingSession
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public TrainingCategory Category { get; set; }
        public string? Notes { get; set; }

        // Relación obligatoria con Microcycle
        public int MicrocycleId { get; set; }
        public Microcycle Microcycle { get; set; } = null!;

        // Relación obligatoria con Planning
        public int PlanningId { get; set; }
        public Planning Planning { get; set; } = null!;

        // Coach que creó la sesión
        public int CreatedByUserId { get; set; }
        public User CreatedBy { get; set; } = null!;

        // Plantilla origen (opcional)
        public int? TemplateId { get; set; }
        public TrainingTemplate? Template { get; set; }

        // Atletas asignados
        public ICollection<TrainingSessionAthlete> Athletes { get; set; } = new List<TrainingSessionAthlete>();

        // Series de la sesión
        public ICollection<TrainingSeries> Series { get; set; } = new List<TrainingSeries>();

        // Metadatos
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
