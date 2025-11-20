namespace StriderWebApi.Dto.VO2MaxSuggestion
{
    /// <summary>
    /// DTO de respuesta para una sugerencia de VO2Max
    /// </summary>
    public class VO2MaxSuggestionResponseDto
    {
        public int Id { get; set; }
        public int CoachId { get; set; }
        public string CoachName { get; set; } = string.Empty;
        public string CoachEmail { get; set; } = string.Empty;
        public int AthleteId { get; set; }
        public string AthleteName { get; set; } = string.Empty;
        public string AthleteEmail { get; set; } = string.Empty;
        public string SuggestedVO2Max { get; set; } = string.Empty;
        public string? Message { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime SuggestedAt { get; set; }
        public DateTime? RespondedAt { get; set; }
    }
}

