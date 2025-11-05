using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Data.Repositories.Interfaces
{
    public interface IPeriodRepository
    {
        Task<Period?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Period>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Period>> GetByPlanningIdAsync(int planningId, CancellationToken cancellationToken = default);
        Task<Period> CreateAsync(Period period, CancellationToken cancellationToken = default);
        Task<Period> UpdateAsync(Period period, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

        // Búsquedas específicas
        Task<IEnumerable<Period>> GetPeriodsWithMesocyclesAsync(int planningId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Period>> GetPeriodsWithMicrocyclesAsync(int planningId, CancellationToken cancellationToken = default);
    }
}
