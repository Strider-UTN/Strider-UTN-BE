using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Dto.Microcycle
{
    public class UpdateMicrocycleDto
    {
        public int Sessions { get; set; }
        public decimal Volume { get; set; } // Se actualiza automáticamente, pero puede ajustarse manualmente
        public MicrocycleIntensity Intensity { get; set; }
        public MicrocycleFocus? Focus { get; set; }
    }
}
