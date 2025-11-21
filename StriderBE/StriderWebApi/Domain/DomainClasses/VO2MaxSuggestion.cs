using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Domain.DomainClasses
{
    /// <summary>
    /// Entidad que representa una sugerencia de actualización de VO2Max de un entrenador a un atleta
    /// </summary>
    [Table("VO2MaxSuggestions")]
    public class VO2MaxSuggestion
    {
        [Key]
        public int Id { get; set; }

        // Relación con el entrenador que hace la sugerencia
        [Required]
        public int CoachId { get; set; }

        [ForeignKey(nameof(CoachId))]
        public User Coach { get; set; } = null!;

        // Relación con el atleta al que se le sugiere
        [Required]
        public int AthleteId { get; set; }

        [ForeignKey(nameof(AthleteId))]
        public User Athlete { get; set; } = null!;

        // Valor sugerido de VO2Max (formato mm:ss, ejemplo: "03:30")
        [Required]
        [MaxLength(10)]
        public string SuggestedVO2Max { get; set; } = string.Empty;

        // Mensaje/comentario opcional del entrenador
        public string? Message { get; set; }

        // Estado de la sugerencia
        [Required]
        [MaxLength(50)]
        public VO2MaxSuggestionStatus Status { get; set; } = VO2MaxSuggestionStatus.Pending;

        // Fechas importantes
        [Required]
        public DateTime SuggestedAt { get; set; } = DateTime.UtcNow;

        public DateTime? RespondedAt { get; set; }

        // Metadata
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}

