using StriderWebApi.Dto.Planning;

namespace StriderWebApi.Services.Interfaces
{
    public interface IPlanningService
    {
        Task<PlanningResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<PlanningResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<PlanningResponseDto>> GetByCoachIdAsync(int coachId, CancellationToken cancellationToken = default);
        Task<IEnumerable<PlanningResponseDto>> GetByAthleteIdAsync(int athleteId, CancellationToken cancellationToken = default);
        Task<IEnumerable<PlanningAthleteResponseDto>> GetAssignedAthletesAsync(int planningId, int coachId, CancellationToken cancellationToken = default);
        Task<PlanningResponseDto> CreateAsync(CreatePlanningDto dto, int coachId, CancellationToken cancellationToken = default);
        Task<PlanningResponseDto> UpdateAsync(int id, UpdatePlanningDto dto, int coachId, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, int coachId, CancellationToken cancellationToken = default);

        // Asignación de atletas
        Task<bool> AssignAthletesAsync(int planningId, IEnumerable<int> athleteIds, int coachId, CancellationToken cancellationToken = default);
        Task<bool> RemoveAthleteAsync(int planningId, int athleteId, int coachId, CancellationToken cancellationToken = default);
        Task<bool> AssignAthletesFromGroupAsync(int planningId, int groupId, int coachId, CancellationToken cancellationToken = default); // Crea relaciones individuales

        // Validaciones
        Task<bool> ValidatePlanningAccessAsync(int planningId, int coachId, CancellationToken cancellationToken = default);
        Task<bool> ValidatePlanningExistsAsync(int planningId, CancellationToken cancellationToken = default);
    }
}
