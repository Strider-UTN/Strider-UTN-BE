using StriderWebApi.Dto.VO2MaxSuggestion;

namespace StriderWebApi.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de sugerencias de VO2Max
    /// </summary>
    public interface IVO2MaxSuggestionService
    {
        Task<VO2MaxSuggestionResponseDto> CreateSuggestionAsync(
            CreateVO2MaxSuggestionDto dto,
            CancellationToken cancellationToken = default);

        Task<VO2MaxSuggestionResponseDto> RespondToSuggestionAsync(
            RespondToVO2MaxSuggestionDto dto,
            CancellationToken cancellationToken = default);

        Task<List<VO2MaxSuggestionResponseDto>> GetPendingSuggestionsAsync(
            CancellationToken cancellationToken = default);

        Task<List<VO2MaxSuggestionResponseDto>> GetSuggestionsByAthleteAsync(
            int athleteId,
            CancellationToken cancellationToken = default);
    }
}

