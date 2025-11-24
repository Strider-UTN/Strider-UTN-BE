using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Data.Repositories.Interfaces
{
    public interface IMicrocycleRepository
    {
        Task<Microcycle?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Microcycle>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<List<Microcycle>> GetByMesocycleIdAsync(int mesocycleId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Microcycle>> GetByPeriodIdAsync(int periodId, CancellationToken cancellationToken = default);
        Task<Microcycle> CreateAsync(Microcycle microcycle, CancellationToken cancellationToken = default);
        Task<Microcycle> UpdateAsync(Microcycle microcycle, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

        // Búsquedas específicas
        Task<Microcycle?> GetByPlanningIdAndDateAsync(int planningId, DateTime date, CancellationToken cancellationToken = default);
        Task<IEnumerable<Microcycle>> GetByPeriodIdWithSessionsAsync(int periodId, CancellationToken cancellationToken = default);
        Task<decimal> CalculateTotalVolumeAsync(int microcycleId, CancellationToken cancellationToken = default); // Calcula volumen basado en sesiones
        Task<bool> UpdateVolumeAsync(int microcycleId, decimal volume, CancellationToken cancellationToken = default);
        
        // Métodos optimizados para batch operations
        Task<Dictionary<int, decimal>> CalculateTotalVolumeBatchAsync(List<int> microcycleIds, CancellationToken cancellationToken = default);
        Task UpdateBatchAsync(List<Microcycle> microcycles, CancellationToken cancellationToken = default);
    }
}
