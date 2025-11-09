using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Data.Repositories.Interfaces
{
    public interface IAthleteInjuryRepository
    {
        Task<AthleteInjury?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<List<AthleteInjury>> GetByAthleteIdAsync(int athleteId, CancellationToken cancellationToken = default);
        Task<List<AthleteInjury>> GetByAthleteIdAndStatusAsync(int athleteId, InjuryStatus status, CancellationToken cancellationToken = default);
        Task<AthleteInjury> CreateAsync(AthleteInjury injury, CancellationToken cancellationToken = default);
        Task<AthleteInjury> UpdateAsync(AthleteInjury injury, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
        Task<List<AthleteInjury>> GetRecentInjuriesForCoachAsync(int coachId, CancellationToken cancellationToken = default);
    }
}
