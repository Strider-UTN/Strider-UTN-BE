using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Data.Repositories.Interfaces
{
    public interface ITrainingIntervalRepository
    {
        Task<TrainingInterval?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TrainingInterval>> GetByTrainingSessionIdAsync(int trainingSessionId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TrainingInterval>> GetByTrainingSeriesIdAsync(int trainingSeriesId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TrainingInterval>> GetByTrainingTemplateIdAsync(int templateId, CancellationToken cancellationToken = default);
        Task<TrainingInterval> CreateAsync(TrainingInterval interval, CancellationToken cancellationToken = default);
        Task<int> CreateMultipleAsync(IEnumerable<TrainingInterval> intervals, CancellationToken cancellationToken = default);
        Task<TrainingInterval> UpdateAsync(TrainingInterval interval, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> DeleteByTrainingSessionIdAsync(int trainingSessionId, CancellationToken cancellationToken = default);
    }
}
