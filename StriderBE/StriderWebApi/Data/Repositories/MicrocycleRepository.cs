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
                .Include(m => m.Mesocycle)
                .Include(m => m.Period)
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Microcycle>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await context.Microcycles
                .Include(m => m.Mesocycle)
                .Include(m => m.Period)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Microcycle>> GetByMesocycleIdAsync(int mesocycleId, CancellationToken cancellationToken = default)
        {
            return await context.Microcycles
                .Include(m => m.Mesocycle)
                .Include(m => m.Period)
                .Where(m => m.MesocycleId == mesocycleId)
                .OrderBy(m => m.WeekNumber)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Microcycle>> GetByPeriodIdAsync(int periodId, CancellationToken cancellationToken = default)
        {
            return await context.Microcycles
                .Include(m => m.Mesocycle)
                .Include(m => m.Period)
                .Where(m => m.PeriodId == periodId)
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
                .Where(m => m.PeriodId == periodId)
                .ToListAsync(cancellationToken);
        }

        public async Task<decimal> CalculateTotalVolumeAsync(int microcycleId, CancellationToken cancellationToken = default)
        {
            // Obtener todas las sesiones del microciclo con sus intervalos
            var sessions = await context.TrainingSessions
                .Include(s => s.Intervals)
                .Where(s => s.MicrocycleId == microcycleId)
                .ToListAsync(cancellationToken);

            decimal totalVolumeKm = 0;

            foreach (var session in sessions)
            {
                // Calcular volumen de la sesión sumando las distancias de los intervalos
                decimal sessionVolumeMeters = 0;
                foreach (var interval in session.Intervals)
                {
                    // Distancia total = distancia del intervalo * repeticiones
                    sessionVolumeMeters += interval.Distance * interval.Repetitions;
                }

                // Convertir de metros a kilómetros y sumar
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
