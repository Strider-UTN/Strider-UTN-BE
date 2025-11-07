using Microsoft.EntityFrameworkCore;
using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Data.Repositories
{
    public class TrainingSessionRepository(StriderDbContext context) : ITrainingSessionsRepository
    {
        public async Task<TrainingSession?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await context.TrainingSessions
                .Include(s => s.Planning)
                .Include(s => s.Microcycle)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<TrainingSession>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await context.TrainingSessions
                .Include(s => s.Planning)
                .Include(s => s.Microcycle)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TrainingSession>> GetByPlanningIdAsync(int planningId, CancellationToken cancellationToken = default)
        {
            return await context.TrainingSessions
                .Include(s => s.Microcycle)
                .Include(s => s.Intervals)
                .Include(s => s.Athletes)
                    .ThenInclude(a => a.Athlete)
                .Where(s => s.PlanningId == planningId)
                .OrderBy(s => s.Date)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TrainingSession>> GetByMicrocycleIdAsync(int microcycleId, CancellationToken cancellationToken = default)
        {
            return await context.TrainingSessions
                .Include(s => s.Intervals)
                .Where(s => s.MicrocycleId == microcycleId)
                .OrderBy(s => s.Date)
                .ToListAsync(cancellationToken);
        }

        public async Task<TrainingSession> CreateAsync(TrainingSession trainingSession, CancellationToken cancellationToken = default)
        {
            trainingSession.CreatedAt = DateTime.UtcNow;
            trainingSession.UpdatedAt = DateTime.UtcNow;
            context.TrainingSessions.Add(trainingSession);
            await context.SaveChangesAsync(cancellationToken);
            return trainingSession;
        }

        public async Task<TrainingSession> UpdateAsync(TrainingSession trainingSession, CancellationToken cancellationToken = default)
        {
            trainingSession.UpdatedAt = DateTime.UtcNow;
            context.TrainingSessions.Update(trainingSession);
            await context.SaveChangesAsync(cancellationToken);
            return trainingSession;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var trainingSession = await GetByIdAsync(id, cancellationToken);
            if (trainingSession == null) return false;

            context.TrainingSessions.Remove(trainingSession);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await context.TrainingSessions.AnyAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<TrainingSession>> GetByDateRangeAsync(int planningId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await context.TrainingSessions
                .Include(s => s.Microcycle)
                .Where(s => s.PlanningId == planningId &&
                    s.Date >= startDate && s.Date <= endDate)
                .OrderBy(s => s.Date)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TrainingSession>> GetByDateAsync(int planningId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await context.TrainingSessions
                .Include(s => s.Microcycle)
                .Where(s => s.PlanningId == planningId && s.Date.Date == date.Date)
                .ToListAsync(cancellationToken);
        }

        public async Task<TrainingSession?> GetByIdWithAthletesAsync(int id, CancellationToken cancellationToken = default)
        {
            var session = await context.TrainingSessions
                .Include(s => s.Athletes)
                    .ThenInclude(a => a.Athlete)
                .Include(s => s.Intervals)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

            // Ordenar intervalos por OrderIndex después de cargarlos
            if (session?.Intervals != null)
            {
                session.Intervals = session.Intervals.OrderBy(i => i.OrderIndex).ToList();
            }

            return session;
        }

        public async Task<IEnumerable<TrainingSession>> GetByAthleteIdAsync(int athleteId, int? planningId = null, CancellationToken cancellationToken = default)
        {
            var query = context.TrainingSessions
                .Include(s => s.Planning)
                .Include(s => s.Microcycle)
                .Include(s => s.Athletes)
                .Include(s => s.Intervals)
                .Where(s => s.Athletes.Any(a => a.AthleteId == athleteId));

            if (planningId.HasValue)
            {
                query = query.Where(s => s.PlanningId == planningId.Value);
            }

            return await query
                .OrderBy(s => s.Date)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> HasSessionsByMicrocycleIdAsync(
            int microcycleId,
            CancellationToken cancellationToken)
        {
            return await context.TrainingSessions
                .AnyAsync(ts => ts.MicrocycleId == microcycleId, cancellationToken);
        }
    }
}
