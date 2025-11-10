using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Dto.Planning
{
    public class PlanningResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public PlanningStatus Status { get; set; }
        public int CoachId { get; set; }
        public string CoachName { get; set; } = string.Empty;
        public int AthletesCount { get; set; }
        public int MesocyclesCount { get; set; }
        public int PeriodsCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
