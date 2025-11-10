using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Data.Repositories.Interfaces
{
    public interface ITrainingSeriesRepository
    {
        Task<TrainingSeries?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TrainingSeries>> GetBySessionIdAsync(int sessionId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TrainingSeries>> GetByTemplateIdAsync(int templateId, CancellationToken cancellationToken = default);

        Task<TrainingSeries> CreateAsync(TrainingSeries series, CancellationToken cancellationToken = default);
        Task CreateManyAsync(IEnumerable<TrainingSeries> series, CancellationToken cancellationToken = default);

        Task<TrainingSeries> UpdateAsync(TrainingSeries series, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task DeleteBySessionIdAsync(int sessionId, CancellationToken cancellationToken = default);
        Task DeleteByTemplateIdAsync(int templateId, CancellationToken cancellationToken = default);
    }
}
