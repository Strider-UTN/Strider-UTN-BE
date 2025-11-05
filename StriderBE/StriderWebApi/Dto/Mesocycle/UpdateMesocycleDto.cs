using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Dto.Mesocycle
{
    public class UpdateMesocycleDto
    {
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int WeeksCount { get; set; }
        public string? Objective { get; set; }
        public MesocycleStatus Status { get; set; }
        public int? PeriodId { get; set; }
    }
}
