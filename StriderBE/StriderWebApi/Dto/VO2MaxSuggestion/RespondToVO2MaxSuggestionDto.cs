using System.ComponentModel.DataAnnotations;

namespace StriderWebApi.Dto.VO2MaxSuggestion
{
    /// <summary>
    /// DTO para responder a una sugerencia de VO2Max
    /// </summary>
    public class RespondToVO2MaxSuggestionDto
    {
        /// <summary>
        /// ID de la sugerencia
        /// </summary>
        [Required]
        public int SuggestionId { get; set; }

        /// <summary>
        /// Indica si se acepta (true) o rechaza (false) la sugerencia
        /// </summary>
        [Required]
        public bool Accept { get; set; }
    }
}

