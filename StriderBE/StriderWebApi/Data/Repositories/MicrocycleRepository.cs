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
    }
}
