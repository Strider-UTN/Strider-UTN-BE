using StriderWebApi.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace StriderWebApi.Domain.DomainClasses
{
    [Table("Plannings")]
    public class Planning
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; } // null = indefinido
        public PlanningStatus Status { get; set; } = PlanningStatus.Draft;
        public int CoachId { get; set; } // ID del entrenador que creó la planificación
        public User Coach { get; set; } = null!;

        // Relaciones con atletas (asignación siempre individual)
        // Nota: Los grupos solo se usan para facilitar la asignación, pero la relación es siempre individual
        public ICollection<PlanningAthlete> PlanningAthletes { get; set; } = new List<PlanningAthlete>();

        // Relaciones con períodos (opcional - para agrupación y reportes)
        public ICollection<Period> Periods { get; set; } = new List<Period>();

        // Relaciones con mesociclos (directa - estructura principal)
        public ICollection<Mesocycle> Mesocycles { get; set; } = new List<Mesocycle>();

        // Relaciones con sesiones de entrenamiento (directa)
        public ICollection<TrainingSession> TrainingSessions { get; set; } = new List<TrainingSession>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
