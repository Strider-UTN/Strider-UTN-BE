using StriderWebApi.Dto.Trainings;

namespace StriderWebApi.Services.Interfaces
{
    public interface ITrainingSessionService
    {
        Task<TrainingSessionResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TrainingSessionResponseDto>> GetByPlanningIdAsync(int planningId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TrainingSessionResponseDto>> GetByMicrocycleIdAsync(int microcycleId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TrainingSessionResponseDto>> GetByMesocycleIdAsync(int mesocycleId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TrainingSessionResponseDto>> GetByAthleteIdAsync(int athleteId, int? planningId = null, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<TrainingSessionResponseDto>> GetMyTrainingSessionsAsync(int athleteId, DateTime? date = null, CancellationToken cancellationToken = default);
        Task<TrainingSessionResponseDto> CreateAsync(CreateTrainingSessionDto dto, int coachId, CancellationToken cancellationToken = default);
        Task<TrainingSessionResponseDto> UpdateAsync(int id, UpdateTrainingSessionDto dto, int coachId, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, int coachId, CancellationToken cancellationToken = default);

        // Funcionalidad especial: Identificación automática de microciclo
        Task<TrainingSessionResponseDto> CreateWithAutoMicrocycleDetectionAsync(CreateTrainingSessionDto dto, int coachId, CancellationToken cancellationToken = default);

        // Validaciones
        Task<bool> ValidateSessionAccessAsync(int sessionId, int coachId, CancellationToken cancellationToken = default);
        Task<bool> ValidateDateInMicrocycleRangeAsync(int microcycleId, DateTime date, CancellationToken cancellationToken = default);
    }
}
