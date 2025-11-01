using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace StriderWebApi.Domain
{
    /// <summary>
    /// Representa la relación entre un entrenador y un atleta
    /// </summary>
    [Table("CoachAthleteRelationships")]
    public class CoachAthleteRelationship
    {
        public int Id { get; set; }

        // Relación con el entrenador
        public int CoachId { get; set; }
        public User Coach { get; set; } = null!;

        // Relación con el atleta
        public int AthleteId { get; set; }
        public User Athlete { get; set; } = null!;

        // Estado de la relación
        public CoachAthleteRelationshipStatus Status { get; set; } = CoachAthleteRelationshipStatus.Pending;

        // Mensaje de invitación opcional
        public string? InvitationMessage { get; set; }

        // Fechas importantes
        public DateTime InvitedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RespondedAt { get; set; }
        public DateTime? LinkedSince { get; set; } // Cuando se aceptó la invitación

        // Metadata
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
