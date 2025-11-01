using Microsoft.EntityFrameworkCore;
using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Data.Repositories
{
    public class TrainingSessionsRepository : ITrainingSessionsRepository
    {
        private readonly StriderDbContext _context;

        public TrainingSessionsRepository(StriderDbContext context)
        {
            _context = context;
        }

        public async Task<TrainingSession?> GetTrainingSessionByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.TrainingSessions
                .Include(s => s.CreatedBy)
                .Include(s => s.Template)
                .Include(s => s.Athletes)
                    .ThenInclude(a => a.Athlete)
                .Include(s => s.Intervals.OrderBy(i => i.OrderIndex))
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<List<TrainingSession>> GetAllTrainingSessionsAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.TrainingSessions
                .Include(s => s.CreatedBy)
                .Include(s => s.Template)
                .Include(s => s.Athletes)
                    .ThenInclude(a => a.Athlete)
                .Include(s => s.Intervals.OrderBy(i => i.OrderIndex))
                .Where(s => s.CreatedByUserId == userId)
                .OrderByDescending(s => s.Date)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<TrainingSession>> GetTrainingSessionsByDateAsync(DateTime date, int userId, CancellationToken cancellationToken = default)
        {
            return await _context.TrainingSessions
                .Include(s => s.CreatedBy)
                .Include(s => s.Template)
                .Include(s => s.Athletes)
                    .ThenInclude(a => a.Athlete)
                .Include(s => s.Intervals.OrderBy(i => i.OrderIndex))
                .Where(s => s.CreatedByUserId == userId && s.Date.Date == date.Date)
                .OrderBy(s => s.Date)
                .ToListAsync(cancellationToken);
        }

        public async Task<TrainingSession> CreateTrainingSessionAsync(TrainingSession trainingSession, CancellationToken cancellationToken = default)
        {
            _context.TrainingSessions.Add(trainingSession);
            await _context.SaveChangesAsync(cancellationToken);
            return trainingSession;
        }

        public async Task<TrainingSession> UpdateTrainingSessionAsync(TrainingSession trainingSession, CancellationToken cancellationToken = default)
        {
            _context.TrainingSessions.Update(trainingSession);
            await _context.SaveChangesAsync(cancellationToken);
            return trainingSession;
        }

        public async Task<bool> DeleteTrainingSessionAsync(int id, CancellationToken cancellationToken = default)
        {
            var deletedRows = await _context.TrainingSessions
                .Where(s => s.Id == id)
                .ExecuteDeleteAsync(cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return deletedRows > 0;
        }
    }
}
