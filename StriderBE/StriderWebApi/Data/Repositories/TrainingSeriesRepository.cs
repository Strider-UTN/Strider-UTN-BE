using Microsoft.EntityFrameworkCore;
using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Data.Repositories
{
    public class TrainingSeriesRepository(StriderDbContext context) : ITrainingSeriesRepository
    {
        public async Task<TrainingSeries?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await context.TrainingSeries
                .Include(s => s.Intervals.OrderBy(i => i.OrderIndex))
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<TrainingSeries>> GetBySessionIdAsync(int sessionId, CancellationToken cancellationToken = default)
        {
            return await context.TrainingSeries
                .Include(s => s.Intervals.OrderBy(i => i.OrderIndex))
                .Where(s => s.TrainingSessionId == sessionId)
                .OrderBy(s => s.OrderIndex)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TrainingSeries>> GetByTemplateIdAsync(int templateId, CancellationToken cancellationToken = default)
        {
            return await context.TrainingSeries
                .Include(s => s.Intervals.OrderBy(i => i.OrderIndex))
                .Where(s => s.TrainingTemplateId == templateId)
                .OrderBy(s => s.OrderIndex)
                .ToListAsync(cancellationToken);
        }

        public async Task<TrainingSeries> CreateAsync(TrainingSeries series, CancellationToken cancellationToken = default)
        {
            series.CreatedAt = DateTime.UtcNow;
            series.UpdatedAt = DateTime.UtcNow;
            context.TrainingSeries.Add(series);
            await context.SaveChangesAsync(cancellationToken);
            return series;
        }

        public async Task CreateManyAsync(IEnumerable<TrainingSeries> series, CancellationToken cancellationToken = default)
        {
            var utcNow = DateTime.UtcNow;
            foreach (var s in series)
            {
                s.CreatedAt = utcNow;
                s.UpdatedAt = utcNow;
            }
            await context.TrainingSeries.AddRangeAsync(series, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<TrainingSeries> UpdateAsync(TrainingSeries series, CancellationToken cancellationToken = default)
        {
            series.UpdatedAt = DateTime.UtcNow;
            context.TrainingSeries.Update(series);
            await context.SaveChangesAsync(cancellationToken);
            return series;
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var series = await context.TrainingSeries.FindAsync([id], cancellationToken);
            if (series == null) return;

            context.TrainingSeries.Remove(series);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteBySessionIdAsync(int sessionId, CancellationToken cancellationToken = default)
        {
            await context.TrainingSeries
                .Where(s => s.TrainingSessionId == sessionId)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task DeleteByTemplateIdAsync(int templateId, CancellationToken cancellationToken = default)
        {
            await context.TrainingSeries
                .Where(s => s.TrainingTemplateId == templateId)
                .ExecuteDeleteAsync(cancellationToken);
        }
    }
}
