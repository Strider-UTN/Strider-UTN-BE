using Google;
using Microsoft.EntityFrameworkCore;
using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain;
using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Data.Repositories
{
    /// <summary>
    /// Implementación del repositorio de relaciones entre coaches y atletas
    /// </summary>
    public class CoachAthleteRelationshipRepository(StriderDbContext context) : ICoachAthleteRelationshipRepository
    {
        public async Task<CoachAthleteRelationship> CreateRelationshipAsync(CoachAthleteRelationship relationship, CancellationToken cancellationToken = default)
        {
            context.CoachAthleteRelationships.Add(relationship);
            await context.SaveChangesAsync(cancellationToken);
            return relationship;
        }

        public async Task<CoachAthleteRelationship?> GetRelationshipByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await context.CoachAthleteRelationships
                .Include(r => r.Coach)
                .Include(r => r.Athlete)
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public async Task<CoachAthleteRelationship?> GetRelationshipByCoachAndAthleteAsync(int coachId, int athleteId, CancellationToken cancellationToken = default)
        {
            return await context.CoachAthleteRelationships
                .Include(r => r.Coach)
                .Include(r => r.Athlete)
                .FirstOrDefaultAsync(
                    r => r.CoachId == coachId && r.AthleteId == athleteId,
                    cancellationToken);
        }

        public async Task<List<CoachAthleteRelationship>> GetRelationshipsByAthleteAsync(int athleteId, CoachAthleteRelationshipStatus? status = null, CancellationToken cancellationToken = default)
        {
            var query = context.CoachAthleteRelationships
                .Include(r => r.Coach)
                .Include(r => r.Athlete)
                .Where(r => r.AthleteId == athleteId);

            if (status.HasValue)
            {
                query = query.Where(r => r.Status == status.Value);
            }

            return await query
                .OrderByDescending(r => r.InvitedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<CoachAthleteRelationship>> GetRelationshipsByCoachAsync(int coachId, CoachAthleteRelationshipStatus? status = null, CancellationToken cancellationToken = default)
        {
            var query = context.CoachAthleteRelationships
                .Include(r => r.Coach)
                .Include(r => r.Athlete)
                .Where(r => r.CoachId == coachId);

            if (status.HasValue)
            {
                query = query.Where(r => r.Status == status.Value);
            }

            return await query
                .OrderByDescending(r => r.InvitedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<CoachAthleteRelationship>> GetPendingInvitationsByAthleteAsync(int athleteId, CancellationToken cancellationToken = default)
        {
            return await context.CoachAthleteRelationships
                .Include(r => r.Coach)
                .Include(r => r.Athlete)
                .Where(r => r.AthleteId == athleteId && r.Status == CoachAthleteRelationshipStatus.Pending)
                .OrderByDescending(r => r.InvitedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<CoachAthleteRelationship> UpdateRelationshipAsync(CoachAthleteRelationship relationship, CancellationToken cancellationToken = default)
        {
            relationship.UpdatedAt = DateTime.UtcNow;
            context.CoachAthleteRelationships.Update(relationship);
            await context.SaveChangesAsync(cancellationToken);
            return relationship;
        }

        public async Task<bool> DeleteRelationshipAsync(int id, CancellationToken cancellationToken = default)
        {
            var relationship = await context.CoachAthleteRelationships
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

            if (relationship == null)
            {
                return false;
            }

            context.CoachAthleteRelationships.Remove(relationship);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> RelationshipExistsAsync(int coachId, int athleteId, CancellationToken cancellationToken = default)
        {
            return await context.CoachAthleteRelationships
                .AnyAsync(
                    r => r.CoachId == coachId && r.AthleteId == athleteId,
                    cancellationToken);
        }
    }
}
