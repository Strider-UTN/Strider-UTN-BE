using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Data.Repositories.Interfaces
{
    public interface ITrainingTemplateRepository
    {
        Task AddTrainingTemplateAsync(TrainingTemplate trainingTemplate, CancellationToken cancellationToken);
        Task<TrainingTemplate?> GetTrainingTemplateByIdAsync(int id, CancellationToken cancellationToken);
        Task<List<TrainingTemplate>> GetAllTrainingTemplatesAsync(int userId, CancellationToken cancellationToken);
        Task<bool> DeleteTrainingTemplateAsync(int id, CancellationToken cancellationToken);
        Task UpdateTrainingTemplateAsync(TrainingTemplate trainingTemplate, CancellationToken cancellationToken);

        Task DeleteSeriesByTemplateIdAsync(int templateId, CancellationToken cancellationToken);
        Task DeleteSeriesBySessionIdAsync(int sessionId, CancellationToken cancellationToken);
        Task AddSeriesAsync(IEnumerable<TrainingSeries> series, CancellationToken cancellationToken);
        Task ReplaceSeriesForTemplateAsync(int templateId, IEnumerable<TrainingSeries> series, CancellationToken cancellationToken);
        Task ReplaceSeriesForSessionAsync(int sessionId, IEnumerable<TrainingSeries> series, CancellationToken cancellationToken);
        Task<List<TrainingSeries>> GetSeriesByTemplateIdAsync(int templateId, CancellationToken cancellationToken);
        Task<List<TrainingSeries>> GetSeriesBySessionIdAsync(int sessionId, CancellationToken cancellationToken);
    }
}
