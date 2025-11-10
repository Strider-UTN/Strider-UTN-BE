using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Dto.Mesocycle
{
    public class CreateMesocycleDto
    {
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public int WeeksCount { get; set; } // IMPORTANTE: Define cuántos microciclos crear automáticamente
        public string? Objective { get; set; }
        public MesocycleStatus Status { get; set; } = MesocycleStatus.Planning;
        public int? PeriodId { get; set; } // Opcional: puede asignarse a un período
    }
}
