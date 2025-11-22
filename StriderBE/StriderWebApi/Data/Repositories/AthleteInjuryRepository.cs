using Microsoft.EntityFrameworkCore;
using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Data.Repositories
{
    public class AthleteInjuryRepository : IAthleteInjuryRepository
    {
        private readonly StriderDbContext _context;

        public AthleteInjuryRepository(StriderDbContext context)
        {
            _context = context;
        }

        public async Task<AthleteInjury?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.AthleteInjuries
                .Include(injury => injury.Athlete)
                .FirstOrDefaultAsync(injury => injury.Id == id, cancellationToken);
        }

        public async Task<List<AthleteInjury>> GetByAthleteIdAsync(int athleteId, CancellationToken cancellationToken = default)
        {
            return await _context.AthleteInjuries
                .Where(injury => injury.AthleteId == athleteId)
                .OrderByDescending(injury => injury.DiagnosisDate)
                .ThenByDescending(injury => injury.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<AthleteInjury>> GetByAthleteIdAndStatusAsync(int athleteId, InjuryStatus status, CancellationToken cancellationToken = default)
        {
            return await _context.AthleteInjuries
                .Where(injury => injury.AthleteId == athleteId && injury.Status == status)
                .OrderByDescending(injury => injury.DiagnosisDate)
                .ThenByDescending(injury => injury.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<AthleteInjury> CreateAsync(AthleteInjury injury, CancellationToken cancellationToken = default)
        {
            await _context.AthleteInjuries.AddAsync(injury, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return injury;
        }

        public async Task<AthleteInjury> UpdateAsync(AthleteInjury injury, CancellationToken cancellationToken = default)
        {
            _context.AthleteInjuries.Update(injury);
            await _context.SaveChangesAsync(cancellationToken);
            return injury;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var injury = await _context.AthleteInjuries.FindAsync(new object[] { id }, cancellationToken);
            if (injury == null)
            {
                return false;
            }

            _context.AthleteInjuries.Remove(injury);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.AthleteInjuries.AnyAsync(injury => injury.Id == id, cancellationToken);
        }

        public async Task<List<AthleteInjury>> GetRecentInjuriesForCoachAsync(int coachId, CancellationToken cancellationToken = default)
        {
            return await _context.AthleteInjuries
                .Include(injury => injury.Athlete)
                .Where(injury => injury.Status == InjuryStatus.Active || injury.Status == InjuryStatus.UnderTreatment)
                .Where(injury => _context.CoachAthleteRelationships
                    .Any(relationship => relationship.CoachId == coachId
                        && relationship.AthleteId == injury.AthleteId
                        && relationship.Status == CoachAthleteRelationshipStatus.Accepted))
                .OrderByDescending(injury => injury.DiagnosisDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<AthleteInjury>> GetTop3RecentInjuriesForAthleteAsync(int athleteId, CancellationToken cancellationToken = default)
        {
            // Primero obtener todas las lesiones del atleta
            var allInjuries = await _context.AthleteInjuries
                .Include(injury => injury.Athlete)
                .Where(injury => injury.AthleteId == athleteId)
                .ToListAsync(cancellationToken);

            // Separar activas/tratamiento del resto
            var activeInjuries = allInjuries
                .Where(injury => injury.Status == InjuryStatus.Active || injury.Status == InjuryStatus.UnderTreatment)
                .OrderByDescending(injury => injury.CreatedAt)
                .ToList();

            var otherInjuries = allInjuries
                .Where(injury => injury.Status != InjuryStatus.Active && injury.Status != InjuryStatus.UnderTreatment)
                .OrderByDescending(injury => injury.CreatedAt)
                .ToList();

            // Priorizar activas, luego agregar del resto hasta completar 3
            var result = new List<AthleteInjury>();
            result.AddRange(activeInjuries.Take(3));
            
            if (result.Count < 3)
            {
                var remainingCount = 3 - result.Count;
                result.AddRange(otherInjuries.Take(remainingCount));
            }

            return result.Take(3).ToList();
        }
    }
}
