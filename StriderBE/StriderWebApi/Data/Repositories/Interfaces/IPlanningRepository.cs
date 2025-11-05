using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Data.Repositories.Interfaces
{
    public interface IPlanningRepository
    {
        Task<Planning?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Planning>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Planning>> GetByCoachIdAsync(int coachId, CancellationToken cancellationToken = default);
        Task<Planning> CreateAsync(Planning planning, CancellationToken cancellationToken = default);
        Task<Planning> UpdateAsync(Planning planning, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

        // Búsquedas específicas
        Task<IEnumerable<Planning>> GetByAthleteIdAsync(int athleteId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Planning>> GetActivePlanningsAsync(int coachId, CancellationToken cancellationToken = default);
        Task<Planning?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    }
}
