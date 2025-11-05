using StriderWebApi.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace StriderWebApi.Domain.DomainClasses
{
    [Table("Periods")]
    public class Period
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int StartWeek { get; set; }
        public int EndWeek { get; set; }
        public string? Objective { get; set; }
        public PeriodStatus Status { get; set; } = PeriodStatus.Planning;

        // Relación opcional con Planning (para contexto)
        public int? PlanningId { get; set; }
        public Planning? Planning { get; set; }

        // Relaciones one-to-many con mesociclos (opcional - un mesociclo puede pertenecer a un período)
        public ICollection<Mesocycle> Mesocycles { get; set; } = new List<Mesocycle>();

        // Relaciones one-to-many con microciclos (requerida - un microciclo debe pertenecer a un período)
        public ICollection<Microcycle> Microcycles { get; set; } = new List<Microcycle>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
