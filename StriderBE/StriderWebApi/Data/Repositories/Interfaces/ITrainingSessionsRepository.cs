using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Data.Repositories.Interfaces
{
    public interface ITrainingSessionsRepository
    {
        Task<TrainingSession?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TrainingSession>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<TrainingSession>> GetByPlanningIdAsync(int planningId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TrainingSession>> GetByMicrocycleIdAsync(int microcycleId, CancellationToken cancellationToken = default);
        Task<TrainingSession> CreateAsync(TrainingSession trainingSession, CancellationToken cancellationToken = default);
        Task<TrainingSession> UpdateAsync(TrainingSession trainingSession, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> HasSessionsByMicrocycleIdAsync(int microcycleId, CancellationToken cancellationToken);

        // Búsquedas específicas
        Task<IEnumerable<TrainingSession>> GetByDateRangeAsync(int planningId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<IEnumerable<TrainingSession>> GetByDateAsync(int planningId, DateTime date, CancellationToken cancellationToken = default);
        Task<TrainingSession?> GetByIdWithAthletesAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TrainingSession>> GetByAthleteIdAsync(int athleteId, int? planningId = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<TrainingSession>> GetByAthleteIdAndDateAsync(int athleteId, DateTime date, CancellationToken cancellationToken = default);
        
        // Método optimizado para obtener conteo de sesiones por múltiples microciclos
        Task<Dictionary<int, int>> GetSessionsCountByMicrocycleIdsAsync(List<int> microcycleIds, CancellationToken cancellationToken = default);
    }
}
