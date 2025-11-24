using StriderWebApi.Dto.Microcycle;

namespace StriderWebApi.Services.Interfaces
{
    public interface IMicrocycleService
    {
        Task<MicrocycleResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<MicrocycleResponseDto>> GetByMesocycleIdAsync(int mesocycleId, CancellationToken cancellationToken = default);
        Task<IEnumerable<MicrocycleResponseDto>> GetByPeriodIdAsync(int periodId, CancellationToken cancellationToken = default);
        Task<IEnumerable<MicrocycleResponseDto>> GetByPlanningIdAsync(int planningId, CancellationToken cancellationToken = default);
        Task<MicrocycleResponseDto> UpdateAsync(int id, UpdateMicrocycleDto dto, int coachId, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, int coachId, CancellationToken cancellationToken = default);

        // Funcionalidad especial
        Task<decimal> RecalculateVolumeAsync(int microcycleId, CancellationToken cancellationToken = default); // Recalcula volumen basado en sesiones
        Task<bool> UpdateVolumeAutomaticallyAsync(int microcycleId, CancellationToken cancellationToken = default); // Actualiza volumen automáticamente

        // Validaciones
        Task<bool> ValidateMicrocycleAccessAsync(int microcycleId, int coachId, CancellationToken cancellationToken = default);
    }
}
