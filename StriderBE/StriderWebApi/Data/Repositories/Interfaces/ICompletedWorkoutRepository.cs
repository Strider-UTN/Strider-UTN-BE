using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Data.Repositories.Interfaces
{
    public interface ICompletedWorkoutRepository
    {
        Task<CompletedWorkout?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<CompletedWorkout> CreateAsync(CompletedWorkout completedWorkout, CancellationToken cancellationToken = default);
        Task<CompletedWorkout> UpdateAsync(CompletedWorkout completedWorkout, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
        
        // Búsquedas por TrainingSessionAthlete
        Task<IEnumerable<CompletedWorkout>> GetByTrainingSessionAthleteIdAsync(int trainingSessionAthleteId, CancellationToken cancellationToken = default);
        Task<CompletedWorkout?> GetByTrainingSessionAthleteIdAndDateAsync(int trainingSessionAthleteId, DateTime date, CancellationToken cancellationToken = default);
        
        // Búsquedas por Athlete
        Task<IEnumerable<CompletedWorkout>> GetByAthleteIdAsync(int athleteId, CancellationToken cancellationToken = default);
        Task<IEnumerable<CompletedWorkout>> GetByAthleteIdAndDateAsync(int athleteId, DateTime date, CancellationToken cancellationToken = default);
        Task<IEnumerable<CompletedWorkout>> GetByAthleteIdAndDateRangeAsync(int athleteId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        
        // Búsquedas por TrainingSession
        Task<IEnumerable<CompletedWorkout>> GetByTrainingSessionIdAsync(int trainingSessionId, CancellationToken cancellationToken = default);
        
        // Búsquedas para Coach con filtros
        Task<IEnumerable<CompletedWorkout>> GetForCoachWithFiltersAsync(
            int coachId,
            int? planningId = null,
            int? trainingGroupId = null,
            int? athleteId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            bool? hasFeedback = null,
            CancellationToken cancellationToken = default);
    }
}

