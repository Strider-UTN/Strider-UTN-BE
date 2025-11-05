using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Dto.Mesocycle
{
    public class MesocycleResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Objective { get; set; }
        public int WeeksCount { get; set; }
        public MesocycleStatus Status { get; set; }
        public int PlanningId { get; set; }
        public int? PeriodId { get; set; }
        public int MicrocyclesCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
