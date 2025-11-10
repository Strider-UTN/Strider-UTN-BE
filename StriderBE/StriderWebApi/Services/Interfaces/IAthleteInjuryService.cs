using StriderWebApi.Dto.Injuries;

namespace StriderWebApi.Services.Interfaces
{
    public interface IAthleteInjuryService
    {
        Task<AthleteInjurySummaryDto> CreateAsync(int athleteId, CreateAthleteInjuryDto dto, CancellationToken cancellationToken = default);
        Task<AthleteInjurySummaryDto?> GetByIdAsync(int injuryId, int athleteId, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<AthleteInjurySummaryDto>> GetByAthleteAsync(int athleteId, CancellationToken cancellationToken = default);
        Task<AthleteInjurySummaryDto> UpdateAsync(int athleteId, int injuryId, UpdateAthleteInjuryDto dto, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<CoachRecentInjuryDto>> GetRecentInjuriesForCoachAsync(int coachId, CancellationToken cancellationToken = default);
    }
}
