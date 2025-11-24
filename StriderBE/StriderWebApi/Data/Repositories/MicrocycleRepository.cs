using Microsoft.EntityFrameworkCore;
using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Data.Repositories
{
    public class MicrocycleRepository(StriderDbContext context) : IMicrocycleRepository
    {
        public async Task<Microcycle?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await context.Microcycles
                .Include(m => m.TrainingSessions)
                .Include(m => m.Mesocycle)
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Microcycle>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await context.Microcycles
                .Include(m => m.TrainingSessions)
                .Include(m => m.Mesocycle)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Microcycle>> GetByMesocycleIdAsync(int mesocycleId, CancellationToken cancellationToken = default)
        {
            return await context.Microcycles
                .Include(m => m.Mesocycle)
                .Where(m => m.MesocycleId == mesocycleId)
                .OrderBy(m => m.WeekNumber)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Microcycle>> GetByPeriodIdAsync(int periodId, CancellationToken cancellationToken = default)
        {
            return await context.Microcycles
                .Include(m => m.Mesocycle)
                .Where(m => m.Mesocycle.PeriodId == periodId)
                .OrderBy(m => m.StartDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<Microcycle> CreateAsync(Microcycle microcycle, CancellationToken cancellationToken = default)
        {
            microcycle.CreatedAt = DateTime.UtcNow;
            microcycle.UpdatedAt = DateTime.UtcNow;
            context.Microcycles.Add(microcycle);
            await context.SaveChangesAsync(cancellationToken);
            return microcycle;
        }

        public async Task<Microcycle> UpdateAsync(Microcycle microcycle, CancellationToken cancellationToken = default)
        {
            microcycle.UpdatedAt = DateTime.UtcNow;
            context.Microcycles.Update(microcycle);
            await context.SaveChangesAsync(cancellationToken);
            return microcycle;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var microcycle = await GetByIdAsync(id, cancellationToken);
            if (microcycle == null) return false;

            context.Microcycles.Remove(microcycle);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await context.Microcycles.AnyAsync(m => m.Id == id, cancellationToken);
        }

        public async Task<Microcycle?> GetByPlanningIdAndDateAsync(int planningId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await context.Microcycles
                .Include(m => m.Mesocycle)
                    .ThenInclude(me => me.Planning)
                .Where(m => m.Mesocycle.PlanningId == planningId &&
                    m.StartDate <= date && m.EndDate >= date)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<Microcycle>> GetByPeriodIdWithSessionsAsync(int periodId, CancellationToken cancellationToken = default)
        {
            return await context.Microcycles
                .Include(m => m.TrainingSessions)
                .Where(m => m.Mesocycle.PeriodId == periodId)
                .ToListAsync(cancellationToken);
        }

        public async Task<decimal> CalculateTotalVolumeAsync(int microcycleId, CancellationToken cancellationToken = default)
        {
            // Obtener todas las sesiones del microciclo con sus series e intervalos
            var sessions = await context.TrainingSessions
                .Include(s => s.Series)
                    .ThenInclude(series => series.Intervals)
                .Where(s => s.MicrocycleId == microcycleId)
                .ToListAsync(cancellationToken);

            decimal totalVolumeKm = 0;

            foreach (var session in sessions)
            {
                decimal sessionVolumeMeters = 0;

                if (session.Series != null)
                {
                    foreach (var series in session.Series)
                    {
                        if (series.Intervals == null) continue;

                        foreach (var interval in series.Intervals)
                        {
                            sessionVolumeMeters += interval.Distance * interval.Repetitions;
                        }
                    }
                }

                totalVolumeKm += sessionVolumeMeters / 1000m;
            }

            return totalVolumeKm;
        }

        public async Task<bool> UpdateVolumeAsync(int microcycleId, decimal volume, CancellationToken cancellationToken = default)
        {
            var microcycle = await GetByIdAsync(microcycleId, cancellationToken);
            if (microcycle == null) return false;

            microcycle.Volume = volume;
            await UpdateAsync(microcycle, cancellationToken);
            return true;
        }

        public async Task<Dictionary<int, decimal>> CalculateTotalVolumeBatchAsync(List<int> microcycleIds, CancellationToken cancellationToken = default)
        {
            if (!microcycleIds.Any())
            {
                return new Dictionary<int, decimal>();
            }

            // Obtener todas las sesiones de todos los microciclos en una sola consulta
            var allSessions = await context.TrainingSessions
                .Where(s => microcycleIds.Contains(s.MicrocycleId))
                .Include(s => s.Series)
                    .ThenInclude(series => series.Intervals)
                .ToListAsync(cancellationToken);

            // Agrupar por microciclo y calcular volumen
            var result = new Dictionary<int, decimal>();
            var sessionsByMicrocycle = allSessions.GroupBy(s => s.MicrocycleId);

            foreach (var group in sessionsByMicrocycle)
            {
                decimal totalVolumeMeters = 0;
                foreach (var session in group)
                {
                    if (session.Series != null)
                    {
                        foreach (var series in session.Series)
                        {
                            if (series.Intervals != null)
                            {
                                foreach (var interval in series.Intervals)
                                {
                                    totalVolumeMeters += interval.Distance * interval.Repetitions * series.Repetitions;
                                }
                            }
                        }
                    }
                }

                result[group.Key] = totalVolumeMeters / 1000m; // Convertir a km
            }

            // Inicializar con 0 para microciclos sin sesiones
            foreach (var microcycleId in microcycleIds)
            {
                if (!result.ContainsKey(microcycleId))
                {
                    result[microcycleId] = 0;
                }
            }

            return result;
        }

        public async Task UpdateBatchAsync(List<Microcycle> microcycles, CancellationToken cancellationToken = default)
        {
            if (!microcycles.Any())
                return;

            foreach (var microcycle in microcycles)
            {
                microcycle.UpdatedAt = DateTime.UtcNow;
                context.Microcycles.Update(microcycle);
            }

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
