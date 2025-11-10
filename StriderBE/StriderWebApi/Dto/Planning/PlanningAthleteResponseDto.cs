using StriderWebApi.Dto.Injuries;

namespace StriderWebApi.Dto.Planning
{
    /// <summary>
    /// DTO de respuesta para un atleta asignado a una planificación
    /// </summary>
    public class PlanningAthleteResponseDto
    {
        public int Id { get; set; } // ID de PlanningAthlete
        public int AthleteId { get; set; }
        public string AthleteName { get; set; } = string.Empty;
        public string AthleteEmail { get; set; } = string.Empty;
        public int PlanningId { get; set; }
        public DateTime AssignedAt { get; set; }
        public bool HasActiveInjury { get; set; }
        public List<AthleteInjurySummaryDto> ActiveInjuries { get; set; } = [];
    }
}
