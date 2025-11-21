using Microsoft.EntityFrameworkCore;
using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Data.Repositories
{
    /// <summary>
    /// Implementación del repositorio de sugerencias de VO2Max
    /// </summary>
    public class VO2MaxSuggestionRepository(StriderDbContext context) : IVO2MaxSuggestionRepository
    {
        public async Task<VO2MaxSuggestion> CreateSuggestionAsync(VO2MaxSuggestion suggestion, CancellationToken cancellationToken = default)
        {
            context.VO2MaxSuggestions.Add(suggestion);
            await context.SaveChangesAsync(cancellationToken);
            return suggestion;
        }

        public async Task<VO2MaxSuggestion?> GetSuggestionByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await context.VO2MaxSuggestions
                .Include(s => s.Coach)
                .Include(s => s.Athlete)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<List<VO2MaxSuggestion>> GetPendingSuggestionsByAthleteAsync(int athleteId, CancellationToken cancellationToken = default)
        {
            return await context.VO2MaxSuggestions
                .Include(s => s.Coach)
                .Include(s => s.Athlete)
                .Where(s => s.AthleteId == athleteId && s.Status == VO2MaxSuggestionStatus.Pending)
                .OrderByDescending(s => s.SuggestedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<VO2MaxSuggestion>> GetSuggestionsByAthleteAsync(int athleteId, VO2MaxSuggestionStatus? status = null, CancellationToken cancellationToken = default)
        {
            var query = context.VO2MaxSuggestions
                .Include(s => s.Coach)
                .Include(s => s.Athlete)
                .Where(s => s.AthleteId == athleteId);

            if (status.HasValue)
            {
                query = query.Where(s => s.Status == status.Value);
            }

            return await query
                .OrderByDescending(s => s.SuggestedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<VO2MaxSuggestion>> GetSuggestionsByCoachAsync(int coachId, VO2MaxSuggestionStatus? status = null, CancellationToken cancellationToken = default)
        {
            var query = context.VO2MaxSuggestions
                .Include(s => s.Coach)
                .Include(s => s.Athlete)
                .Where(s => s.CoachId == coachId);

            if (status.HasValue)
            {
                query = query.Where(s => s.Status == status.Value);
            }

            return await query
                .OrderByDescending(s => s.SuggestedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<VO2MaxSuggestion> UpdateSuggestionAsync(VO2MaxSuggestion suggestion, CancellationToken cancellationToken = default)
        {
            suggestion.UpdatedAt = DateTime.UtcNow;
            context.VO2MaxSuggestions.Update(suggestion);
            await context.SaveChangesAsync(cancellationToken);
            return suggestion;
        }
    }
}

