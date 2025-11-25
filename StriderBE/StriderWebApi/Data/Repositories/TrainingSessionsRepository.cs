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
                .Include(s => s.Series)
                    .ThenInclude(series => series.Intervals)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<TrainingSession>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await context.TrainingSessions
                .Include(s => s.Planning)
                .Include(s => s.Microcycle)
                .Include(s => s.Series)
                    .ThenInclude(series => series.Intervals)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TrainingSession>> GetByPlanningIdAsync(int planningId, CancellationToken cancellationToken = default)
        {
            return await context.TrainingSessions
                .Include(s => s.Microcycle)
                .Include(s => s.Series)
                    .ThenInclude(series => series.Intervals)
                .Include(s => s.Athletes)
                    .ThenInclude(a => a.Athlete)
                .Where(s => s.PlanningId == planningId)
                .OrderBy(s => s.Date)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TrainingSession>> GetByMicrocycleIdAsync(int microcycleId, CancellationToken cancellationToken = default)
        {
            return await context.TrainingSessions
                .Include(s => s.Series)
                    .ThenInclude(series => series.Intervals)
                .Include(s => s.Athletes)
                    .ThenInclude(a => a.Athlete)
                .Where(s => s.MicrocycleId == microcycleId)
                .OrderBy(s => s.Date)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TrainingSession>> GetByMesocycleIdAsync(int mesocycleId, CancellationToken cancellationToken = default)
        {
            return await context.TrainingSessions
                .Include(s => s.Microcycle)
                .Include(s => s.Series)
                    .ThenInclude(series => series.Intervals)
                .Include(s => s.Athletes)
                    .ThenInclude(a => a.Athlete)
                .Where(s => s.Microcycle != null && s.Microcycle.MesocycleId == mesocycleId)
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
                .Include(s => s.Series)
                    .ThenInclude(series => series.Intervals)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

            if (session?.Series != null)
            {
                foreach (var series in session.Series)
                {
                    if (series.Intervals != null)
                    {
                        series.Intervals = series.Intervals
                            .OrderBy(i => i.OrderIndex)
                            .ToList();
                    }
                }

                session.Series = session.Series
                    .OrderBy(s => s.OrderIndex)
                    .ToList();
            }

            return session;
        }

        public async Task<IEnumerable<TrainingSession>> GetByAthleteIdAsync(int athleteId, int? planningId = null, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
        {
            var query = context.TrainingSessions
                .Include(s => s.Planning)
                .Include(s => s.Microcycle)
                .Include(s => s.Athletes)
                    .ThenInclude(a => a.CompletedWorkouts)
                        .ThenInclude(cw => cw.Laps)
                .Include(s => s.Athletes)
                    .ThenInclude(a => a.CompletedWorkouts)
                        .ThenInclude(cw => cw.Injuries)
                .Include(s => s.Series)
                    .ThenInclude(series => series.Intervals)
                .Where(s => s.Athletes.Any(a => a.AthleteId == athleteId));

            if (planningId.HasValue)
            {
                query = query.Where(s => s.PlanningId == planningId.Value);
            }

            // Filtrar por rango de fechas si se proporciona
            // Asegurar que las fechas estén en UTC para PostgreSQL
            if (startDate.HasValue)
            {
                var startDateValue = startDate.Value;
                // Si la fecha viene como Unspecified, asumir que es UTC
                if (startDateValue.Kind == DateTimeKind.Unspecified)
                {
                    startDateValue = DateTime.SpecifyKind(startDateValue.Date, DateTimeKind.Utc);
                }
                else if (startDateValue.Kind == DateTimeKind.Local)
                {
                    startDateValue = startDateValue.Date.ToUniversalTime();
                }
                else
                {
                    startDateValue = startDateValue.Date;
                }
                // Incluir todas las sesiones desde el inicio del día startDate
                query = query.Where(s => s.Date.Date >= startDateValue);
            }

            if (endDate.HasValue)
            {
                var endDateValue = endDate.Value;
                // Si la fecha viene como Unspecified, asumir que es UTC
                if (endDateValue.Kind == DateTimeKind.Unspecified)
                {
                    endDateValue = DateTime.SpecifyKind(endDateValue.Date, DateTimeKind.Utc);
                }
                else if (endDateValue.Kind == DateTimeKind.Local)
                {
                    endDateValue = endDateValue.Date.ToUniversalTime();
                }
                else
                {
                    endDateValue = endDateValue.Date;
                }
                // Incluir todas las sesiones hasta el final del día endDate
                // Usar <= para incluir todo el día endDate
                query = query.Where(s => s.Date.Date <= endDateValue);
            }

            return await query
                .OrderBy(s => s.Date)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TrainingSession>> GetByAthleteIdAndDateAsync(int athleteId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await context.TrainingSessions
                .Include(s => s.Planning)
                .Include(s => s.Microcycle)
                .Include(s => s.Athletes)
                    .ThenInclude(a => a.Athlete)
                .Include(s => s.Athletes)
                    .ThenInclude(a => a.CompletedWorkouts)
                .Include(s => s.Series)
                    .ThenInclude(series => series.Intervals)
                .Where(s => s.Athletes.Any(a => a.AthleteId == athleteId) 
                    && s.Date.Date == date.Date)
                .OrderBy(s => s.Date)
                .ThenBy(s => s.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> HasSessionsByMicrocycleIdAsync(
            int microcycleId,
            CancellationToken cancellationToken)
        {
            return await context.TrainingSessions
                .AnyAsync(ts => ts.MicrocycleId == microcycleId, cancellationToken);
        }

        public async Task<Dictionary<int, int>> GetSessionsCountByMicrocycleIdsAsync(List<int> microcycleIds, CancellationToken cancellationToken = default)
        {
            if (!microcycleIds.Any())
            {
                return new Dictionary<int, int>();
            }

            // Consulta optimizada: solo contar sesiones por microciclo sin cargar datos
            var counts = await context.TrainingSessions
                .Where(s => microcycleIds.Contains(s.MicrocycleId))
                .GroupBy(s => s.MicrocycleId)
                .Select(g => new { MicrocycleId = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            return counts.ToDictionary(x => x.MicrocycleId, x => x.Count);
        }
    }
}
