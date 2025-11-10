using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Dto.Microcycle
{
    public class MicrocycleResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int WeekNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Sessions { get; set; } // Calculado automáticamente desde TrainingSessions
        public decimal Volume { get; set; } // Calculado automáticamente desde TrainingSessions
        public MicrocycleIntensity Intensity { get; set; }
        public MicrocycleFocus? Focus { get; set; }
        public int MesocycleId { get; set; }
        public int TrainingSessionsCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
