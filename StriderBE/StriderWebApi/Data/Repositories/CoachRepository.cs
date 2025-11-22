using Microsoft.EntityFrameworkCore;
using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Data.Repositories
{
    public class CoachRepository : ICoachRepository
    {
        private readonly StriderDbContext _context;

        public CoachRepository(StriderDbContext context)
        {
            _context = context;
        }
        public async Task<Coach?> GetCoachByIdAsync(int id)
        {
            return await _context.Coaches.FindAsync(id);
        }

        public async Task<Coach?> GetCoachByEmailAsync(string email)
        {
            return await _context.Coaches.FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<List<Coach>> GetAllCoachesAsync()
        {
            return await _context.Coaches.ToListAsync();
        }

        public async Task AddCoachAsync(Coach coach)
        {
            await _context.Coaches.AddAsync(coach);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateCoachAsync(Coach coach)
        {
            _context.Coaches.Update(coach);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCoachAsync(Coach coach)
        {
            _context.Coaches.Remove(coach);
            await _context.SaveChangesAsync();
        }

    }
}
