using System.ComponentModel.DataAnnotations;

namespace StriderWebApi.Dto.VO2MaxSuggestion
{
    /// <summary>
    /// DTO para crear una sugerencia de actualización de VO2Max
    /// </summary>
    public class CreateVO2MaxSuggestionDto
    {
        /// <summary>
        /// ID del atleta al que se le sugiere el VO2Max
        /// </summary>
        [Required]
        public int AthleteId { get; set; }

        /// <summary>
        /// Valor sugerido de VO2Max (formato mm:ss, ejemplo: "03:30")
        /// </summary>
        [Required]
        [MaxLength(10)]
        public string SuggestedVO2Max { get; set; } = string.Empty;

        /// <summary>
        /// Mensaje/comentario opcional del entrenador
        /// </summary>
        [MaxLength(500)]
        public string? Message { get; set; }
    }
}

