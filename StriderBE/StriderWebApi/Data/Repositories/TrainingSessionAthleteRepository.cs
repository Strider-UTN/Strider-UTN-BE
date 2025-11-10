using Google;
using Microsoft.EntityFrameworkCore;
using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Data.Repositories
{
    public class TrainingSessionAthleteRepository(StriderDbContext context) : ITrainingSessionAthleteRepository
    {
        public async Task<TrainingSessionAthlete?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await context.TrainingSessionAthletes
                .Include(tsa => tsa.TrainingSession)
                .Include(tsa => tsa.Athlete)
                .FirstOrDefaultAsync(tsa => tsa.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<TrainingSessionAthlete>> GetByTrainingSessionIdAsync(int trainingSessionId, CancellationToken cancellationToken = default)
        {
            return await context.TrainingSessionAthletes
                .Include(tsa => tsa.Athlete)
                .Where(tsa => tsa.TrainingSessionId == trainingSessionId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TrainingSessionAthlete>> GetByAthleteIdAsync(int athleteId, CancellationToken cancellationToken = default)
        {
            return await context.TrainingSessionAthletes
                .Include(tsa => tsa.TrainingSession)
                    .ThenInclude(ts => ts.Microcycle)
                .Include(tsa => tsa.TrainingSession)
                    .ThenInclude(ts => ts.Planning)
                .Where(tsa => tsa.AthleteId == athleteId)
                .ToListAsync(cancellationToken);
        }

        public async Task<TrainingSessionAthlete> CreateAsync(TrainingSessionAthlete trainingSessionAthlete, CancellationToken cancellationToken = default)
        {
            trainingSessionAthlete.AssignedAt = DateTime.UtcNow;
            context.TrainingSessionAthletes.Add(trainingSessionAthlete);
            await context.SaveChangesAsync(cancellationToken);
            return trainingSessionAthlete;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var trainingSessionAthlete = await GetByIdAsync(id, cancellationToken);
            if (trainingSessionAthlete == null) return false;

            context.TrainingSessionAthletes.Remove(trainingSessionAthlete);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteByTrainingSessionIdAsync(int trainingSessionId, CancellationToken cancellationToken = default)
        {
            var trainingSessionAthletes = await GetByTrainingSessionIdAsync(trainingSessionId, cancellationToken);
            context.TrainingSessionAthletes.RemoveRange(trainingSessionAthletes);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteByAthleteIdAsync(int athleteId, CancellationToken cancellationToken = default)
        {
            var trainingSessionAthletes = await GetByAthleteIdAsync(athleteId, cancellationToken);
            context.TrainingSessionAthletes.RemoveRange(trainingSessionAthletes);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> ExistsAsync(int trainingSessionId, int athleteId, CancellationToken cancellationToken = default)
        {
            return await context.TrainingSessionAthletes
                .AnyAsync(tsa => tsa.TrainingSessionId == trainingSessionId && tsa.AthleteId == athleteId, cancellationToken);
        }

        public async Task<int> CreateMultipleAsync(IEnumerable<TrainingSessionAthlete> trainingSessionAthletes, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            foreach (var tsa in trainingSessionAthletes)
            {
                tsa.AssignedAt = now;
            }

            context.TrainingSessionAthletes.AddRange(trainingSessionAthletes);
            await context.SaveChangesAsync(cancellationToken);
            return trainingSessionAthletes.Count();
        }
    }
}
