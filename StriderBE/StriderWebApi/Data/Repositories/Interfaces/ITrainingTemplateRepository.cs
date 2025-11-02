using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Data.Repositories.Interfaces
{
    public interface ITrainingTemplateRepository
    {
        Task AddTrainingTemplateAsync (TrainingTemplate trainingTemplate, CancellationToken cancellationToken);
        Task<TrainingTemplate?> GetTrainingTemplateByIdAsync (int id, CancellationToken cancellationToken);
        Task<List<TrainingTemplate>> GetAllTrainingTemplatesAsync (int userId, CancellationToken cancellationToken);
        Task<bool> DeleteTrainingTemplateAsync (int id, CancellationToken cancellationToken);
        Task UpdateTrainingTemplateAsync(TrainingTemplate trainingTemplate, CancellationToken cancellationToken);
        Task DeleteAllAsociatedIntervalsAsync(int id, CancellationToken cancellationToken);
        Task AddTrainingIntervalsToTemplateAsync(List<TrainingInterval> trainingIntervals, CancellationToken cancellationToken);
    }
}
