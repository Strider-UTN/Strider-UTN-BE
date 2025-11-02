using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Data.Repositories.Interfaces
{
    public interface ITrainingSessionsRepository
    {
        Task<TrainingSession?> GetTrainingSessionByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<List<TrainingSession>> GetAllTrainingSessionsAsync(int userId, CancellationToken cancellationToken = default);
        Task<List<TrainingSession>> GetTrainingSessionsByDateAsync(DateTime date, int userId, CancellationToken cancellationToken = default);
        Task<TrainingSession> CreateTrainingSessionAsync(TrainingSession trainingSession, CancellationToken cancellationToken = default);
        Task<TrainingSession> UpdateTrainingSessionAsync(TrainingSession trainingSession, CancellationToken cancellationToken = default);
        Task<bool> DeleteTrainingSessionAsync(int id, CancellationToken cancellationToken = default);
    }
}
