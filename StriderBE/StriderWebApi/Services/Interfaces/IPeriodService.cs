using StriderWebApi.Dto.Period;

namespace StriderWebApi.Services.Interfaces
{
    public interface IPeriodService
    {
        Task<PeriodResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<PeriodResponseDto>> GetByPlanningIdAsync(int planningId, CancellationToken cancellationToken = default);
        Task<PeriodResponseDto> CreateAsync(CreatePeriodDto dto, int planningId, int coachId, CancellationToken cancellationToken = default);
        Task<PeriodResponseDto> UpdateAsync(int id, UpdatePeriodDto dto, int coachId, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, int coachId, CancellationToken cancellationToken = default);

        // Validaciones
        Task<bool> ValidatePeriodAccessAsync(int periodId, int coachId, CancellationToken cancellationToken = default);
    }
}
