using StriderWebApi.Dto.Mesocycle;

namespace StriderWebApi.Services.Interfaces
{
    public interface IMesocycleService
    {
        Task<MesocycleResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<MesocycleResponseDto>> GetByPlanningIdAsync(int planningId, CancellationToken cancellationToken = default);
        Task<MesocycleResponseDto> CreateAsync(CreateMesocycleDto dto, int planningId, int coachId, CancellationToken cancellationToken = default);
        Task<MesocycleResponseDto> UpdateAsync(int id, UpdateMesocycleDto dto, int coachId, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, int coachId, CancellationToken cancellationToken = default);

        // Funcionalidad especial: Crear mesociclo con microciclos automáticos
        Task<MesocycleResponseDto> CreateWithAutoMicrocyclesAsync(CreateMesocycleDto dto, int planningId, int periodId, int coachId, CancellationToken cancellationToken = default);

        // Validaciones
        Task<bool> ValidateMesocycleAccessAsync(int mesocycleId, int coachId, CancellationToken cancellationToken = default);
        Task<bool> ValidateNoDateOverlapAsync(int planningId, DateTime startDate, DateTime endDate, int? excludeMesocycleId = null, CancellationToken cancellationToken = default);
    }
}
