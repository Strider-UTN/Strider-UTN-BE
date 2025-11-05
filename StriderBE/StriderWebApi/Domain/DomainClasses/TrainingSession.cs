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

        //Relación con Microcycle (REQUERIDA - porque todas las sesiones están dentro de un microciclo)
        public int MicrocycleId { get; set; }
        public Microcycle Microcycle { get; set; } = null!;

        // NUEVO: Relación con Planning (REQUERIDA - para optimización de consultas y validación de integridad)
        public int PlanningId { get; set; }
        public Planning Planning { get; set; } = null!;

        // Relación con usuario/coach que crea la sesión
        public int CreatedByUserId { get; set; }
        public User CreatedBy { get; set; } = null!;

        // Relación opcional con plantilla (si fue creada desde una plantilla)
        public int? TemplateId { get; set; }
        public TrainingTemplate? Template { get; set; }

        // Relaciones muchos a muchos con atletas
        public ICollection<TrainingSessionAthlete> Athletes { get; set; } = new List<TrainingSessionAthlete>();

        // Intervalos de la sesión (usando la misma entidad TrainingInterval)
        public ICollection<TrainingInterval> Intervals { get; set; } = new List<TrainingInterval>();

        // Metadata
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
