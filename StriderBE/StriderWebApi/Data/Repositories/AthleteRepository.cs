using Microsoft.EntityFrameworkCore;
using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;

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
            return await _context.Athletes.FindAsync(id);
        }

        public async Task<Athlete?> GetAthleteByUsernameAsync(string username)
        {
            return await _context.Athletes.FirstOrDefaultAsync(a => a.Username == username);
        }

        public async Task<Athlete?> GetAthleteByEmailAsync(string email)
        {
            return await _context.Athletes.FirstOrDefaultAsync(a => a.Email == email);
        }

        public async Task<List<Athlete>> GetAllAthletesAsync()
        {
            return await _context.Athletes.ToListAsync();
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
