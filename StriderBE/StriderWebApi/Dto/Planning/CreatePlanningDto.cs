using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Dto.Planning
{
    public class CreatePlanningDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public PlanningStatus Status { get; set; } = PlanningStatus.Draft;
        public IEnumerable<int> AthleteIds { get; set; } = new List<int>(); // Atletas individuales
        public IEnumerable<int>? GroupIds { get; set; } // Opcional: grupos para facilitar asignación (se convierten a athleteIds en el servicio)
    }
}
