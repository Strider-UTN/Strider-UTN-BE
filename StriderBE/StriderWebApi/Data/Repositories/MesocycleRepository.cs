using Microsoft.EntityFrameworkCore;
using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Data.Repositories
{
    public class MesocycleRepository(StriderDbContext context) : IMesocycleRepository
    {
        public async Task<Mesocycle?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await context.Mesocycles
                .Include(m => m.Planning)
                .Include(m => m.Period)
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Mesocycle>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await context.Mesocycles
                .Include(m => m.Planning)
                .Include(m => m.Period)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Mesocycle>> GetByPlanningIdAsync(int planningId, CancellationToken cancellationToken = default)
        {
            return await context.Mesocycles
                .Include(m => m.Planning)
                .Include(m => m.Period)
                .Where(m => m.PlanningId == planningId)
                .ToListAsync(cancellationToken);
        }

        public async Task<Mesocycle> CreateAsync(Mesocycle mesocycle, CancellationToken cancellationToken = default)
        {
            mesocycle.CreatedAt = DateTime.UtcNow;
            mesocycle.UpdatedAt = DateTime.UtcNow;
            context.Mesocycles.Add(mesocycle);
            await context.SaveChangesAsync(cancellationToken);
            return mesocycle;
        }

        public async Task<Mesocycle> UpdateAsync(Mesocycle mesocycle, CancellationToken cancellationToken = default)
        {
            mesocycle.UpdatedAt = DateTime.UtcNow;
            context.Mesocycles.Update(mesocycle);
            await context.SaveChangesAsync(cancellationToken);
            return mesocycle;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var mesocycle = await GetByIdAsync(id, cancellationToken);
            if (mesocycle == null) return false;

            context.Mesocycles.Remove(mesocycle);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await context.Mesocycles.AnyAsync(m => m.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Mesocycle>> GetByPeriodIdAsync(int periodId, CancellationToken cancellationToken = default)
        {
            return await context.Mesocycles
                .Include(m => m.Planning)
                .Include(m => m.Period)
                .Where(m => m.PeriodId == periodId)
                .ToListAsync(cancellationToken);
        }

        public async Task<Mesocycle?> GetByIdWithMicrocyclesAsync(int id, CancellationToken cancellationToken = default)
        {
            return await context.Mesocycles
                .Include(m => m.Microcycles)
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Mesocycle>> GetByPlanningIdWithMicrocyclesAsync(int planningId, CancellationToken cancellationToken = default)
        {
            return await context.Mesocycles
                .Include(m => m.Microcycles)
                .Where(m => m.PlanningId == planningId)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> HasOverlappingDatesAsync(int planningId, DateTime startDate, DateTime endDate, int? excludeMesocycleId = null, CancellationToken cancellationToken = default)
        {
            var query = context.Mesocycles
                .Where(m => m.PlanningId == planningId &&
                    ((m.StartDate <= startDate && m.EndDate >= startDate) ||
                     (m.StartDate <= endDate && m.EndDate >= endDate) ||
                     (m.StartDate >= startDate && m.EndDate <= endDate)));

            if (excludeMesocycleId.HasValue)
            {
                query = query.Where(m => m.Id != excludeMesocycleId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }
    }
}
