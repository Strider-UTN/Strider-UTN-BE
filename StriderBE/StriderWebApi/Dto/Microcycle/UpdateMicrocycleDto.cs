using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Dto.Microcycle
{
    public class UpdateMicrocycleDto
    {
        public string Name { get; set; } = string.Empty; // ✅ NUEVO
        public string? Description { get; set; } // ✅ NUEVO (opcional)
                                                 // Sessions y Volume NO deben estar aquí - se calculan automáticamente desde las sesiones
        public MicrocycleIntensity Intensity { get; set; }
        public MicrocycleFocus? Focus { get; set; }
    }
}
