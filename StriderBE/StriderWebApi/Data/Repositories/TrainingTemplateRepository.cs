using Microsoft.EntityFrameworkCore;
using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Data.Repositories
{
    public class TrainingTemplateRepository(StriderDbContext context) : ITrainingTemplateRepository
    {
        public async Task AddTrainingTemplateAsync(TrainingTemplate trainingTemplate, CancellationToken cancellationToken)
        {
            await context.TrainingTemplates.AddAsync(trainingTemplate, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<TrainingTemplate?> GetTrainingTemplateByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await context.TrainingTemplates
                .Include(t => t.Series)
                    .ThenInclude(series => series.Intervals)
                .AsTracking()
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<List<TrainingTemplate>> GetAllTrainingTemplatesAsync(int userId, CancellationToken cancellationToken)
        {
            return await context.TrainingTemplates
                .Include(t => t.Series)
                    .ThenInclude(series => series.Intervals)
                .Where(t => t.CreatedByUserId == userId)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> DeleteTrainingTemplateAsync(int id, CancellationToken cancellationToken)
        {
            var deletedTemplates = await context.TrainingTemplates
                .Where(t => t.Id == id)
                .ExecuteDeleteAsync(cancellationToken);

            if (deletedTemplates > 0)
            {
                await context.SaveChangesAsync(cancellationToken);
                return true;
            }

            return false;
        }

        public async Task UpdateTrainingTemplateAsync(TrainingTemplate trainingTemplate, CancellationToken cancellationToken)
        {
            if (context.Entry(trainingTemplate).State == EntityState.Detached)
            {
                context.TrainingTemplates.Update(trainingTemplate);
            }

            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteSeriesByTemplateIdAsync(int templateId, CancellationToken cancellationToken)
        {
            await context.TrainingSeries
                .Where(s => s.TrainingTemplateId == templateId)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task DeleteSeriesBySessionIdAsync(int sessionId, CancellationToken cancellationToken)
        {
            await context.TrainingSeries
                .Where(s => s.TrainingSessionId == sessionId)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task AddSeriesAsync(IEnumerable<TrainingSeries> series, CancellationToken cancellationToken)
        {
            var seriesList = series.ToList();
            if (!seriesList.Any())
            {
                return;
            }

            var now = DateTime.UtcNow;
            foreach (var trainingSeries in seriesList)
            {
                trainingSeries.CreatedAt = now;
                trainingSeries.UpdatedAt = now;

                if (trainingSeries.Intervals != null)
                {
                    foreach (var interval in trainingSeries.Intervals)
                    {
                        interval.CreatedAt = now;
                        interval.UpdatedAt = now;
                    }
                }
            }

            await context.TrainingSeries.AddRangeAsync(seriesList, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task ReplaceSeriesForTemplateAsync(int templateId, IEnumerable<TrainingSeries> series, CancellationToken cancellationToken)
        {
            await DeleteSeriesByTemplateIdAsync(templateId, cancellationToken);
            await AddSeriesAsync(series, cancellationToken);
        }

        public async Task ReplaceSeriesForSessionAsync(int sessionId, IEnumerable<TrainingSeries> series, CancellationToken cancellationToken)
        {
            await DeleteSeriesBySessionIdAsync(sessionId, cancellationToken);
            await AddSeriesAsync(series, cancellationToken);
        }

        public async Task<List<TrainingSeries>> GetSeriesByTemplateIdAsync(int templateId, CancellationToken cancellationToken)
        {
            return await context.TrainingSeries
                .Include(s => s.Intervals)
                .Where(s => s.TrainingTemplateId == templateId)
                .OrderBy(s => s.OrderIndex)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<TrainingSeries>> GetSeriesBySessionIdAsync(int sessionId, CancellationToken cancellationToken)
        {
            return await context.TrainingSeries
                .Include(s => s.Intervals)
                .Where(s => s.TrainingSessionId == sessionId)
                .OrderBy(s => s.OrderIndex)
                .ToListAsync(cancellationToken);
        }
    }
}
