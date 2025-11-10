using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Dto.Period
{
    public class PeriodResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int StartWeek { get; set; }
        public int EndWeek { get; set; }
        public string? Objective { get; set; }
        public PeriodStatus Status { get; set; }
        public int? PlanningId { get; set; }
        public int MesocyclesCount { get; set; }
        public int MicrocyclesCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
