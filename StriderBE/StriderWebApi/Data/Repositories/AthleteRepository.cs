using Microsoft.EntityFrameworkCore;
using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;
using System;
using System.Linq;

namespace StriderWebApi.Data.Repositories
{
    public class AthleteRepository : IAthleteRepository
    {
        private readonly StriderDbContext _context;

        public AthleteRepository(StriderDbContext context)
        {
            _context = context;
        }

        public async Task<Athlete?> GetAthleteByIdAsync(int id)
        {
            return await _context.Athletes
                .Include(a => a.Injuries)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Athlete?> GetAthleteByUsernameAsync(string username)
        {
            return await _context.Athletes
                .Include(a => a.Injuries)
                .FirstOrDefaultAsync(a => a.Username == username);
        }

        public async Task<Athlete?> GetAthleteByEmailAsync(string email)
        {
            return await _context.Athletes
                .Include(a => a.Injuries)
                .FirstOrDefaultAsync(a => a.Email == email);
        }

        public async Task<List<Athlete>> GetAllAthletesAsync()
        {
            return await _context.Athletes
                .Include(a => a.Injuries)
                .ToListAsync();
        }

        public async Task<List<Athlete>> GetByIdsWithDetailsAsync(IEnumerable<int> athleteIds, CancellationToken cancellationToken = default)
        {
            var ids = athleteIds.Distinct().ToList();
            if (ids.Count == 0)
            {
                return new List<Athlete>();
            }

            return await _context.Athletes
                .Include(a => a.Injuries)
                .Where(a => ids.Contains(a.Id))
                .ToListAsync(cancellationToken);
        }

        public async Task<bool?> GetActiveStatusAsync(int athleteId, CancellationToken cancellationToken = default)
        {
            return await _context.Athletes
                .AsNoTracking()
                .Where(a => a.Id == athleteId)
                .Select(a => (bool?)a.Active)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> UpdateActiveStatusAsync(int athleteId, bool isActive, CancellationToken cancellationToken = default)
        {
            var athlete = await _context.Athletes.FirstOrDefaultAsync(a => a.Id == athleteId, cancellationToken);
            if (athlete == null)
            {
                return false;
            }

            athlete.Active = isActive;
            athlete.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task AddAthleteAsync(Athlete athlete)
        {
            await _context.Athletes.AddAsync(athlete);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAthleteAsync(Athlete athlete)
        {
            _context.Athletes.Update(athlete);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAthleteAsync(Athlete athlete)
        {
            _context.Athletes.Remove(athlete);
            await _context.SaveChangesAsync();
        }
    }
}
