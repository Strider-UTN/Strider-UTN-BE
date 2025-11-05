using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Dto.Microcycle
{
    public class MicrocycleResponseDto
    {
        public int Id { get; set; }
        public int WeekNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Sessions { get; set; }
        public decimal Volume { get; set; }
        public MicrocycleIntensity Intensity { get; set; }
        public MicrocycleFocus? Focus { get; set; }
        public int MesocycleId { get; set; }
        public int PeriodId { get; set; }
        public int TrainingSessionsCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
