using Microsoft.EntityFrameworkCore;
using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Data.Repositories
{
    public class PlanningAthleteRepository(StriderDbContext context) : IPlanningAthleteRepository
    {
        public async Task<PlanningAthlete?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await context.PlanningAthletes
                .Include(pa => pa.Planning)
                .Include(pa => pa.Athlete)
                .FirstOrDefaultAsync(pa => pa.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<PlanningAthlete>> GetByPlanningIdAsync(int planningId, CancellationToken cancellationToken = default)
        {
            return await context.PlanningAthletes
                .Include(pa => pa.Athlete)
                .Where(pa => pa.PlanningId == planningId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<PlanningAthlete>> GetByAthleteIdAsync(int athleteId, CancellationToken cancellationToken = default)
        {
            return await context.PlanningAthletes
                .Include(pa => pa.Planning)
                .Where(pa => pa.AthleteId == athleteId)
                .ToListAsync(cancellationToken);
        }

        public async Task<PlanningAthlete> CreateAsync(PlanningAthlete planningAthlete, CancellationToken cancellationToken = default)
        {
            planningAthlete.CreatedAt = DateTime.UtcNow;
            context.PlanningAthletes.Add(planningAthlete);
            await context.SaveChangesAsync(cancellationToken);
            return planningAthlete;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var planningAthlete = await GetByIdAsync(id, cancellationToken);
            if (planningAthlete == null) return false;

            context.PlanningAthletes.Remove(planningAthlete);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> ExistsAsync(int planningId, int athleteId, CancellationToken cancellationToken = default)
        {
            return await context.PlanningAthletes
                .AnyAsync(pa => pa.PlanningId == planningId && pa.AthleteId == athleteId, cancellationToken);
        }

        public async Task<int> CreateMultipleAsync(IEnumerable<PlanningAthlete> planningAthletes, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            foreach (var pa in planningAthletes)
            {
                pa.CreatedAt = now;
            }

            context.PlanningAthletes.AddRange(planningAthletes);
            await context.SaveChangesAsync(cancellationToken);
            return planningAthletes.Count();
        }

        public async Task<bool> DeleteByPlanningIdAsync(int planningId, CancellationToken cancellationToken = default)
        {
            var planningAthletes = await GetByPlanningIdAsync(planningId, cancellationToken);
            context.PlanningAthletes.RemoveRange(planningAthletes);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteByAthleteIdAsync(int athleteId, CancellationToken cancellationToken = default)
        {
            var planningAthletes = await GetByAthleteIdAsync(athleteId, cancellationToken);
            context.PlanningAthletes.RemoveRange(planningAthletes);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
