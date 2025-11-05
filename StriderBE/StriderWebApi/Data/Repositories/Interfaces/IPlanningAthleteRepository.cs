using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Data.Repositories.Interfaces
{
    public interface IPlanningAthleteRepository
    {
        Task<PlanningAthlete?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<PlanningAthlete>> GetByPlanningIdAsync(int planningId, CancellationToken cancellationToken = default);
        Task<IEnumerable<PlanningAthlete>> GetByAthleteIdAsync(int athleteId, CancellationToken cancellationToken = default);
        Task<PlanningAthlete> CreateAsync(PlanningAthlete planningAthlete, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int planningId, int athleteId, CancellationToken cancellationToken = default);

        // Operaciones masivas
        Task<int> CreateMultipleAsync(IEnumerable<PlanningAthlete> planningAthletes, CancellationToken cancellationToken = default);
        Task<bool> DeleteByPlanningIdAsync(int planningId, CancellationToken cancellationToken = default);
        Task<bool> DeleteByAthleteIdAsync(int athleteId, CancellationToken cancellationToken = default);
    }
}
