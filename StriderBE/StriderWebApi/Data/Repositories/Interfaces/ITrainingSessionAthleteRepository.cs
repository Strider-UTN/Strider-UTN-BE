using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Data.Repositories.Interfaces
{
    public interface ITrainingSessionAthleteRepository
    {
        Task<TrainingSessionAthlete?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TrainingSessionAthlete>> GetByTrainingSessionIdAsync(int trainingSessionId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TrainingSessionAthlete>> GetByAthleteIdAsync(int athleteId, CancellationToken cancellationToken = default);
        Task<TrainingSessionAthlete> CreateAsync(TrainingSessionAthlete trainingSessionAthlete, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> DeleteByTrainingSessionIdAsync(int trainingSessionId, CancellationToken cancellationToken = default);
        Task<bool> DeleteByAthleteIdAsync(int athleteId, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int trainingSessionId, int athleteId, CancellationToken cancellationToken = default);
        Task<int> CreateMultipleAsync(IEnumerable<TrainingSessionAthlete> trainingSessionAthletes, CancellationToken cancellationToken = default);
    }
}
