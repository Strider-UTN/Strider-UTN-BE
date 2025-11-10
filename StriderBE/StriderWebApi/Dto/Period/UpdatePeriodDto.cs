using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Dto.Period
{
    public class UpdatePeriodDto
    {
        public string Name { get; set; } = string.Empty;
        public int StartWeek { get; set; }
        public int EndWeek { get; set; }
        public string? Objective { get; set; }
        public PeriodStatus Status { get; set; }
    }
}
