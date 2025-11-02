using Microsoft.EntityFrameworkCore;
using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Data.Repositories
{
    public class TrainingTemplateRepository : ITrainingTemplateRepository
    {
        private readonly StriderDbContext _context;

        public TrainingTemplateRepository(StriderDbContext context)
        {
            _context = context;
        }
        public async Task AddTrainingTemplateAsync(TrainingTemplate trainingTemplate, CancellationToken cancellationToken)
        {
            // Guardar en la base de datos
            await _context.TrainingTemplates.AddAsync(trainingTemplate, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<TrainingTemplate?> GetTrainingTemplateByIdAsync(int id, CancellationToken cancellationToken)
        {
            // Cargar la plantilla con sus intervalos para retornar
            return await _context.TrainingTemplates
                .Include(t => t.Intervals.OrderBy(i => i.OrderIndex))
                .AsTracking()
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }
        public async Task<List<TrainingTemplate>> GetAllTrainingTemplatesAsync(int userId, CancellationToken cancellationToken)
        {
            // Cargar las plantillas del usuario con sus intervalos para retornar
            return await _context.TrainingTemplates
                .Include(t => t.Intervals.OrderBy(i => i.OrderIndex))
                .Where(t => t.CreatedByUserId == userId)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> DeleteTrainingTemplateAsync(int id, CancellationToken cancellationToken)
        {
            var deletedTemplates = await _context.TrainingTemplates
                .Where(t => t.Id == id)
                .ExecuteDeleteAsync(cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return deletedTemplates > 0;
        }

        public async Task UpdateTrainingTemplateAsync(TrainingTemplate trainingTemplate, CancellationToken cancellationToken)
        {
            // Si la entidad no está trackeada, marcarla como modificada
            if (_context.Entry(trainingTemplate).State == EntityState.Detached)
            {
                _context.TrainingTemplates.Update(trainingTemplate);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAllAsociatedIntervalsAsync(int id, CancellationToken cancellationToken)
        {
            await _context.TrainingIntervals
                    .Where(i => i.TrainingTemplateId == id)
                    .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task AddTrainingIntervalsToTemplateAsync(List<TrainingInterval> trainingIntervals, CancellationToken cancellationToken)
        {
            await _context.TrainingIntervals.AddRangeAsync(trainingIntervals, cancellationToken);
            await _context.SaveChangesAsync();
        }
    }
}
