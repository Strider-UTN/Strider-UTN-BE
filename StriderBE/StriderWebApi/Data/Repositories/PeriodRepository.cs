using Microsoft.EntityFrameworkCore;
using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Data.Repositories
{
    public class PeriodRepository(StriderDbContext context) : IPeriodRepository
    {
        public async Task<Period?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await context.Periods
                .Include(p => p.Planning)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Period>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await context.Periods
                .Include(p => p.Planning)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Period>> GetByPlanningIdAsync(int planningId, CancellationToken cancellationToken = default)
        {
            return await context.Periods
                .Include(p => p.Planning)
                .Where(p => p.PlanningId == planningId)
                .ToListAsync(cancellationToken);
        }

        public async Task<Period> CreateAsync(Period period, CancellationToken cancellationToken = default)
        {
            period.CreatedAt = DateTime.UtcNow;
            period.UpdatedAt = DateTime.UtcNow;
            context.Periods.Add(period);
            await context.SaveChangesAsync(cancellationToken);
            return period;
        }

        public async Task<Period> UpdateAsync(Period period, CancellationToken cancellationToken = default)
        {
            period.UpdatedAt = DateTime.UtcNow;
            context.Periods.Update(period);
            await context.SaveChangesAsync(cancellationToken);
            return period;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var period = await GetByIdAsync(id, cancellationToken);
            if (period == null) return false;

            context.Periods.Remove(period);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await context.Periods.AnyAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Period>> GetPeriodsWithMesocyclesAsync(int planningId, CancellationToken cancellationToken = default)
        {
            return await context.Periods
                .Include(p => p.Mesocycles)
                .Where(p => p.PlanningId == planningId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Period>> GetPeriodsWithMicrocyclesAsync(int planningId, CancellationToken cancellationToken = default)
        {
            return await context.Periods
                .Include(p => p.Mesocycles)
                    .ThenInclude(m => m.Microcycles)
                .Where(p => p.PlanningId == planningId)
                .ToListAsync(cancellationToken);
        }
    }
}
