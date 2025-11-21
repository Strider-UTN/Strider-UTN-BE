using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Data.Repositories.Interfaces
{
    /// <summary>
    /// Interfaz para el repositorio de sugerencias de VO2Max
    /// </summary>
    public interface IVO2MaxSuggestionRepository
    {
        Task<VO2MaxSuggestion> CreateSuggestionAsync(VO2MaxSuggestion suggestion, CancellationToken cancellationToken = default);
        Task<VO2MaxSuggestion?> GetSuggestionByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<List<VO2MaxSuggestion>> GetPendingSuggestionsByAthleteAsync(int athleteId, CancellationToken cancellationToken = default);
        Task<List<VO2MaxSuggestion>> GetSuggestionsByAthleteAsync(int athleteId, VO2MaxSuggestionStatus? status = null, CancellationToken cancellationToken = default);
        Task<List<VO2MaxSuggestion>> GetSuggestionsByCoachAsync(int coachId, VO2MaxSuggestionStatus? status = null, CancellationToken cancellationToken = default);
        Task<VO2MaxSuggestion> UpdateSuggestionAsync(VO2MaxSuggestion suggestion, CancellationToken cancellationToken = default);
    }
}

