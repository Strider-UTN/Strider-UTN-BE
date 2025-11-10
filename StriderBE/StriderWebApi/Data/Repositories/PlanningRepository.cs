using Microsoft.EntityFrameworkCore;
using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Data.Repositories
{
    public class PlanningRepository(StriderDbContext context) : IPlanningRepository
    {
        public async Task<Planning?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await context.Plannings
                .Include(p => p.Coach)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Planning>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await context.Plannings
                .Include(p => p.Coach)
                .Include(p => p.PlanningAthletes)
                .Include(p => p.Mesocycles)
                    .ThenInclude(m => m.Microcycles)
                        .ThenInclude(mc => mc.TrainingSessions)
                .Include(p => p.Periods)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Planning>> GetByCoachIdAsync(int coachId, CancellationToken cancellationToken = default)
        {
            return await context.Plannings
                .Include(p => p.Coach)
                .Include(p => p.PlanningAthletes)
                .Include(p => p.Mesocycles)
                    .ThenInclude(m => m.Microcycles)
                        .ThenInclude(mc => mc.TrainingSessions)
                .Include(p => p.Periods)
                .Where(p => p.CoachId == coachId)
                .ToListAsync(cancellationToken);
        }

        public async Task<Planning> CreateAsync(Planning planning, CancellationToken cancellationToken = default)
        {
            planning.CreatedAt = DateTime.UtcNow;
            planning.UpdatedAt = DateTime.UtcNow;
            context.Plannings.Add(planning);
            await context.SaveChangesAsync(cancellationToken);
            return planning;
        }

        public async Task<Planning> UpdateAsync(Planning planning, CancellationToken cancellationToken = default)
        {
            planning.UpdatedAt = DateTime.UtcNow;
            context.Plannings.Update(planning);
            await context.SaveChangesAsync(cancellationToken);
            return planning;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var planning = await GetByIdAsync(id, cancellationToken);
            if (planning == null) return false;

            context.Plannings.Remove(planning);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await context.Plannings.AnyAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Planning>> GetByAthleteIdAsync(int athleteId, CancellationToken cancellationToken = default)
        {
            return await context.Plannings
                .Include(p => p.Coach)
                .Include(p => p.PlanningAthletes)
                .Include(p => p.Mesocycles)
                    .ThenInclude(m => m.Microcycles)
                        .ThenInclude(mc => mc.TrainingSessions)
                .Include(p => p.Periods)
                .Where(p => p.PlanningAthletes.Any(pa => pa.AthleteId == athleteId))
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Planning>> GetActivePlanningsAsync(int coachId, CancellationToken cancellationToken = default)
        {
            return await context.Plannings
                .Include(p => p.Coach)
                .Include(p => p.PlanningAthletes)
                .Include(p => p.Mesocycles)
                    .ThenInclude(m => m.Microcycles)
                        .ThenInclude(mc => mc.TrainingSessions)
                .Include(p => p.Periods)
                .Where(p => p.CoachId == coachId && p.Status == PlanningStatus.Active)
                .ToListAsync(cancellationToken);
        }

        public async Task<Planning?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await context.Plannings
                .Include(p => p.Coach)
                .Include(p => p.PlanningAthletes)
                    .ThenInclude(pa => pa.Athlete)
                .Include(p => p.Mesocycles)
                .Include(p => p.Periods)
                .Include(p => p.TrainingSessions)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }
    }
}
