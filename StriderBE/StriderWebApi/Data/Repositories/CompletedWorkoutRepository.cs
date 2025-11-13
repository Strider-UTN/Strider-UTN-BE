using Microsoft.EntityFrameworkCore;
using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Data.Repositories
{
    public class CompletedWorkoutRepository(StriderDbContext context) : ICompletedWorkoutRepository
    {
        public async Task<CompletedWorkout?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await context.CompletedWorkouts
                .Include(w => w.TrainingSessionAthlete)
                    .ThenInclude(tsa => tsa.TrainingSession)
                        .ThenInclude(ts => ts.Planning)
                .Include(w => w.TrainingSessionAthlete)
                    .ThenInclude(tsa => tsa.Athlete)
                .Include(w => w.Sensations)
                .Include(w => w.Laps.OrderBy(l => l.Index))
                .Include(w => w.Injuries)
                .Include(w => w.Feedback)
                    .ThenInclude(f => f!.Coach)
                .Include(w => w.Feedback)
                    .ThenInclude(f => f!.LapFeedbacks)
                .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
        }

        public async Task<CompletedWorkout> CreateAsync(CompletedWorkout completedWorkout, CancellationToken cancellationToken = default)
        {
            completedWorkout.CreatedAt = DateTime.UtcNow;
            context.CompletedWorkouts.Add(completedWorkout);
            await context.SaveChangesAsync(cancellationToken);
            return completedWorkout;
        }

        public async Task<CompletedWorkout> UpdateAsync(CompletedWorkout completedWorkout, CancellationToken cancellationToken = default)
        {
            completedWorkout.UpdatedAt = DateTime.UtcNow;
            context.CompletedWorkouts.Update(completedWorkout);
            await context.SaveChangesAsync(cancellationToken);
            return completedWorkout;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var completedWorkout = await GetByIdAsync(id);
            if (completedWorkout == null) return false;

            context.CompletedWorkouts.Remove(completedWorkout);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await context.CompletedWorkouts.AnyAsync(w => w.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<CompletedWorkout>> GetByTrainingSessionAthleteIdAsync(int trainingSessionAthleteId, CancellationToken cancellationToken = default)
        {
            return await context.CompletedWorkouts
                .Include(w => w.TrainingSessionAthlete)
                    .ThenInclude(tsa => tsa.TrainingSession)
                        .ThenInclude(ts => ts.Planning)
                .Include(w => w.TrainingSessionAthlete)
                    .ThenInclude(tsa => tsa.Athlete)
                .Include(w => w.Sensations)
                .Include(w => w.Laps.OrderBy(l => l.Index))
                .Include(w => w.Injuries)
                .Include(w => w.Feedback)
                    .ThenInclude(f => f!.Coach)
                .Include(w => w.Feedback)
                    .ThenInclude(f => f!.LapFeedbacks)
                .Where(w => w.TrainingSessionAthleteId == trainingSessionAthleteId)
                .OrderByDescending(w => w.Date)
                .ThenByDescending(w => w.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<CompletedWorkout?> GetByTrainingSessionAthleteIdAndDateAsync(int trainingSessionAthleteId, DateTime date, CancellationToken cancellationToken = default)
        {
            // Asegurar que la fecha esté en UTC para PostgreSQL
            var dateUtc = date.Kind == DateTimeKind.Utc ? date : DateTime.SpecifyKind(date, DateTimeKind.Utc);
            var dateOnly = DateOnly.FromDateTime(dateUtc);
            
            return await context.CompletedWorkouts
                .Include(w => w.TrainingSessionAthlete)
                    .ThenInclude(tsa => tsa.TrainingSession)
                        .ThenInclude(ts => ts.Planning)
                .Include(w => w.TrainingSessionAthlete)
                    .ThenInclude(tsa => tsa.Athlete)
                .Include(w => w.Sensations)
                .Include(w => w.Laps.OrderBy(l => l.Index))
                .Include(w => w.Injuries)
                .Include(w => w.Feedback)
                    .ThenInclude(f => f!.Coach)
                .Include(w => w.Feedback)
                    .ThenInclude(f => f!.LapFeedbacks)
                .Where(w => w.TrainingSessionAthleteId == trainingSessionAthleteId 
                    && DateOnly.FromDateTime(w.Date) == dateOnly)
                .OrderByDescending(w => w.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<CompletedWorkout>> GetByAthleteIdAsync(int athleteId, CancellationToken cancellationToken = default)
        {
            return await context.CompletedWorkouts
                .Include(w => w.TrainingSessionAthlete)
                    .ThenInclude(tsa => tsa.TrainingSession)
                        .ThenInclude(ts => ts.Planning)
                .Include(w => w.TrainingSessionAthlete)
                    .ThenInclude(tsa => tsa.Athlete)
                .Include(w => w.Sensations)
                .Include(w => w.Laps.OrderBy(l => l.Index))
                .Include(w => w.Injuries)
                .Include(w => w.Feedback)
                    .ThenInclude(f => f!.Coach)
                .Include(w => w.Feedback)
                    .ThenInclude(f => f!.LapFeedbacks)
                .Where(w => w.TrainingSessionAthlete.AthleteId == athleteId)
                .OrderByDescending(w => w.Date)
                .ThenByDescending(w => w.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<CompletedWorkout>> GetByAthleteIdAndDateAsync(int athleteId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await context.CompletedWorkouts
                .Include(w => w.TrainingSessionAthlete)
                    .ThenInclude(tsa => tsa.TrainingSession)
                        .ThenInclude(ts => ts.Planning)
                .Include(w => w.TrainingSessionAthlete)
                    .ThenInclude(tsa => tsa.Athlete)
                .Include(w => w.Sensations)
                .Include(w => w.Laps.OrderBy(l => l.Index))
                .Include(w => w.Injuries)
                .Include(w => w.Feedback)
                    .ThenInclude(f => f!.Coach)
                .Include(w => w.Feedback)
                    .ThenInclude(f => f!.LapFeedbacks)
                .Where(w => w.TrainingSessionAthlete.AthleteId == athleteId 
                    && w.Date.Date == date.Date)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<CompletedWorkout>> GetByAthleteIdAndDateRangeAsync(int athleteId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await context.CompletedWorkouts
                .Include(w => w.TrainingSessionAthlete)
                    .ThenInclude(tsa => tsa.TrainingSession)
                        .ThenInclude(ts => ts.Planning)
                .Include(w => w.TrainingSessionAthlete)
                    .ThenInclude(tsa => tsa.Athlete)
                .Include(w => w.Sensations)
                .Include(w => w.Laps.OrderBy(l => l.Index))
                .Include(w => w.Injuries)
                .Include(w => w.Feedback)
                    .ThenInclude(f => f!.Coach)
                .Include(w => w.Feedback)
                    .ThenInclude(f => f!.LapFeedbacks)
                .Where(w => w.TrainingSessionAthlete.AthleteId == athleteId 
                    && w.Date.Date >= startDate.Date 
                    && w.Date.Date <= endDate.Date)
                .OrderByDescending(w => w.Date)
                .ThenByDescending(w => w.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<CompletedWorkout>> GetByTrainingSessionIdAsync(int trainingSessionId, CancellationToken cancellationToken = default)
        {
            return await context.CompletedWorkouts
                .Include(w => w.TrainingSessionAthlete)
                    .ThenInclude(tsa => tsa.Athlete)
                .Include(w => w.Sensations)
                .Include(w => w.Laps.OrderBy(l => l.Index))
                .Include(w => w.Injuries)
                .Where(w => w.TrainingSessionAthlete.TrainingSessionId == trainingSessionId)
                .OrderByDescending(w => w.Date)
                .ThenByDescending(w => w.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<CompletedWorkout>> GetForCoachWithFiltersAsync(
            int coachId,
            int? planningId = null,
            int? trainingGroupId = null,
            int? athleteId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            bool? hasFeedback = null,
            CancellationToken cancellationToken = default)
        {
            var query = context.CompletedWorkouts
                .Include(w => w.TrainingSessionAthlete)
                    .ThenInclude(tsa => tsa.TrainingSession)
                        .ThenInclude(ts => ts.Planning)
                .Include(w => w.TrainingSessionAthlete)
                    .ThenInclude(tsa => tsa.TrainingSession)
                        .ThenInclude(ts => ts.Microcycle)
                            .ThenInclude(m => m.Mesocycle)
                .Include(w => w.TrainingSessionAthlete)
                    .ThenInclude(tsa => tsa.Athlete)
                .Include(w => w.Sensations)
                .Include(w => w.Laps.OrderBy(l => l.Index))
                .Include(w => w.Injuries)
                .Include(w => w.Feedback)
                    .ThenInclude(f => f!.Coach)
                .Where(w => w.TrainingSessionAthlete.TrainingSession.Planning.CoachId == coachId);

            // Filtrar por planificación
            if (planningId.HasValue)
            {
                query = query.Where(w => w.TrainingSessionAthlete.TrainingSession.PlanningId == planningId.Value);
            }

            // Filtrar por sede (TrainingGroup) - usando join con TrainingGroupMembers
            if (trainingGroupId.HasValue)
            {
                query = query.Where(w => context.TrainingGroupMembers
                    .Any(tgm => tgm.UserId == w.TrainingSessionAthlete.AthleteId 
                        && tgm.TrainingGroupId == trainingGroupId.Value 
                        && tgm.Status == Domain.Enums.TrainingGroupMemberStatus.Active));
            }

            // Filtrar por atleta
            if (athleteId.HasValue)
            {
                query = query.Where(w => w.TrainingSessionAthlete.AthleteId == athleteId.Value);
            }

            // Filtrar por feedback (pendientes o evaluados)
            if (hasFeedback.HasValue)
            {
                if (hasFeedback.Value)
                {
                    // Solo con feedback (evaluados)
                    query = query.Where(w => w.Feedback != null);
                }
                else
                {
                    // Solo sin feedback (pendientes)
                    query = query.Where(w => w.Feedback == null);
                }
            }

            // Filtrar por rango de fechas
            if (startDate.HasValue)
            {
                var startDateUtc = startDate.Value.Kind == DateTimeKind.Utc 
                    ? startDate.Value 
                    : DateTime.SpecifyKind(startDate.Value, DateTimeKind.Utc);
                query = query.Where(w => w.Date >= startDateUtc);
            }

            if (endDate.HasValue)
            {
                var endDateUtc = endDate.Value.Kind == DateTimeKind.Utc 
                    ? endDate.Value 
                    : DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc);
                // Incluir todo el día final
                var endDateEndOfDay = endDateUtc.Date.AddDays(1).AddTicks(-1);
                query = query.Where(w => w.Date <= endDateEndOfDay);
            }

            return await query
                .OrderByDescending(w => w.Date)
                .ThenByDescending(w => w.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}

