using System.ComponentModel.DataAnnotations.Schema;

namespace StriderWebApi.Domain.DomainClasses
{
    [Table("PlanningAthletes")]
    public class PlanningAthlete
    {
        public int Id { get; set; }
        public int PlanningId { get; set; }
        public Planning Planning { get; set; } = null!;
        public int AthleteId { get; set; }
        public Athlete Athlete { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
