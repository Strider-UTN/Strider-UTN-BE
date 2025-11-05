using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Data.Repositories.Interfaces
{
    public interface IMesocycleRepository
    {
        Task<Mesocycle?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Mesocycle>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Mesocycle>> GetByPlanningIdAsync(int planningId, CancellationToken cancellationToken = default);
        Task<Mesocycle> CreateAsync(Mesocycle mesocycle, CancellationToken cancellationToken = default);
        Task<Mesocycle> UpdateAsync(Mesocycle mesocycle, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

        // Búsquedas específicas
        Task<IEnumerable<Mesocycle>> GetByPeriodIdAsync(int periodId, CancellationToken cancellationToken = default);
        Task<Mesocycle?> GetByIdWithMicrocyclesAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Mesocycle>> GetByPlanningIdWithMicrocyclesAsync(int planningId, CancellationToken cancellationToken = default);
        Task<bool> HasOverlappingDatesAsync(int planningId, DateTime startDate, DateTime endDate, int? excludeMesocycleId = null, CancellationToken cancellationToken = default);
    }
}
