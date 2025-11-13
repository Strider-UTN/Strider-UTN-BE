using StriderWebApi.Dto.CompletedWorkout;

namespace StriderWebApi.Services.Interfaces
{
    public interface ICompletedWorkoutService
    {
        Task<CompletedWorkoutResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<CompletedWorkoutResponseDto> CreateAsync(CreateCompletedWorkoutDto dto, int athleteId, CancellationToken cancellationToken = default);
        Task<CompletedWorkoutResponseDto> UpdateAsync(int id, CreateCompletedWorkoutDto dto, int athleteId, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, int athleteId, CancellationToken cancellationToken = default);
        
        // Búsquedas por TrainingSessionAthlete
        Task<IEnumerable<CompletedWorkoutResponseDto>> GetByTrainingSessionAthleteIdAsync(int trainingSessionAthleteId, CancellationToken cancellationToken = default);
        Task<CompletedWorkoutResponseDto?> GetByTrainingSessionAthleteIdAndDateAsync(int trainingSessionAthleteId, DateTime date, CancellationToken cancellationToken = default);
        
        // Búsquedas por Athlete (usuario autenticado)
        Task<IEnumerable<CompletedWorkoutResponseDto>> GetMyCompletedWorkoutsAsync(int athleteId, DateTime? date = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<CompletedWorkoutResponseDto>> GetMyCompletedWorkoutsByDateRangeAsync(int athleteId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        
        // Búsquedas para Coach con filtros (agrupadas por atleta)
        Task<IEnumerable<CompletedWorkoutsGroupedByAthleteDto>> GetForCoachWithFiltersGroupedByAthleteAsync(
            int coachId,
            int? planningId = null,
            int? trainingGroupId = null,
            int? athleteId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            bool? hasFeedback = null,
            CancellationToken cancellationToken = default);
        
        // Validaciones
        Task<bool> ValidateWorkoutAccessAsync(int workoutId, int athleteId, CancellationToken cancellationToken = default);
        
        // Feedback del entrenador
        Task<WorkoutFeedbackResponseDto> SubmitWorkoutFeedbackAsync(int completedWorkoutId, CreateWorkoutFeedbackDto dto, int coachId, CancellationToken cancellationToken = default);
        Task<WorkoutFeedbackResponseDto> UpdateWorkoutFeedbackAsync(int completedWorkoutId, UpdateWorkoutFeedbackDto dto, int coachId, CancellationToken cancellationToken = default);
    }
}

