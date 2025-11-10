using StriderWebApi.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace StriderWebApi.Domain.DomainClasses
{
    [Table("Mesocycles")]
    public class Mesocycle
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Objective { get; set; }
        public int WeeksCount { get; set; }
        public MesocycleStatus Status { get; set; } = MesocycleStatus.Planning;

        // Relación directa con Planning (REQUERIDA - estructura principal)
        public int PlanningId { get; set; }
        public Planning Planning { get; set; } = null!;

        // Relación opcional con Period (para agrupación - un mesociclo pertenece a un solo período)
        public int? PeriodId { get; set; }
        public Period? Period { get; set; }

        // Relaciones con microciclos (un microciclo pertenece a un solo mesociclo)
        public ICollection<Microcycle> Microcycles { get; set; } = new List<Microcycle>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
