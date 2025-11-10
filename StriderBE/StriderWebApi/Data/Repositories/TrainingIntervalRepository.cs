using Google;
using Microsoft.EntityFrameworkCore;
using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Data.Repositories
{
    public class TrainingIntervalRepository(StriderDbContext context) : ITrainingIntervalRepository
    {
        public async Task<TrainingInterval?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await context.TrainingIntervals
                .Include(i => i.TrainingSeries)
                    .ThenInclude(series => series.TrainingSession)
                .Include(i => i.TrainingSeries)
                    .ThenInclude(series => series.TrainingTemplate)
                .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<TrainingInterval>> GetByTrainingSessionIdAsync(int trainingSessionId, CancellationToken cancellationToken = default)
        {
            return await context.TrainingIntervals
                .Include(i => i.TrainingSeries)
                .Where(i => i.TrainingSeries != null && i.TrainingSeries.TrainingSessionId == trainingSessionId)
                .OrderBy(i => i.OrderIndex)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TrainingInterval>> GetByTrainingSeriesIdAsync(int trainingSeriesId, CancellationToken cancellationToken = default)
        {
            return await context.TrainingIntervals
                .Where(i => i.TrainingSeriesId == trainingSeriesId)
                .OrderBy(i => i.OrderIndex)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TrainingInterval>> GetByTrainingTemplateIdAsync(int templateId, CancellationToken cancellationToken = default)
        {
            return await context.TrainingIntervals
                .Include(i => i.TrainingSeries)
                .Where(i => i.TrainingSeries != null && i.TrainingSeries.TrainingTemplateId == templateId)
                .OrderBy(i => i.OrderIndex)
                .ToListAsync(cancellationToken);
        }

        public async Task<TrainingInterval> CreateAsync(TrainingInterval interval, CancellationToken cancellationToken = default)
        {
            interval.CreatedAt = DateTime.UtcNow;
            context.TrainingIntervals.Add(interval);
            await context.SaveChangesAsync(cancellationToken);
            return interval;
        }

        public async Task<int> CreateMultipleAsync(IEnumerable<TrainingInterval> intervals, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            foreach (var interval in intervals)
            {
                interval.CreatedAt = now;
            }

            context.TrainingIntervals.AddRange(intervals);
            await context.SaveChangesAsync(cancellationToken);
            return intervals.Count();
        }

        public async Task<TrainingInterval> UpdateAsync(TrainingInterval interval, CancellationToken cancellationToken = default)
        {
            interval.UpdatedAt = DateTime.UtcNow;
            context.TrainingIntervals.Update(interval);
            await context.SaveChangesAsync(cancellationToken);
            return interval;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var interval = await GetByIdAsync(id, cancellationToken);
            if (interval == null) return false;

            context.TrainingIntervals.Remove(interval);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteByTrainingSessionIdAsync(int trainingSessionId, CancellationToken cancellationToken = default)
        {
            var intervals = await GetByTrainingSessionIdAsync(trainingSessionId, cancellationToken);
            context.TrainingIntervals.RemoveRange(intervals);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
