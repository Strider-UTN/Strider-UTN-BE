using System.ComponentModel.DataAnnotations;

namespace StriderWebApi.Dto.Invitation
{
    /// <summary>
    /// DTO para invitar a un atleta
    /// </summary>
    public class InviteAthleteDto
    {
        /// <summary>
        /// Email del atleta a invitar
        /// </summary>
        public string AthleteEmail { get; set; } = string.Empty;

        /// <summary>
        /// Mensaje opcional de invitación
        /// </summary>
        public string? Message { get; set; }
    }

    /// <summary>
    /// DTO para responder a una invitación
    /// </summary>
    public class RespondToInvitationDto
    {
        /// <summary>
        /// ID de la relación
        /// </summary>
        public int RelationshipId { get; set; }

        /// <summary>
        /// True si acepta la invitación, false si la rechaza
        /// </summary>
        public bool Accept { get; set; }
    }

    /// <summary>
    /// DTO de respuesta para una relación coach-atleta
    /// </summary>
    public class CoachAthleteRelationshipResponseDto
    {
        public int Id { get; set; }
        public int CoachId { get; set; }
        public string CoachName { get; set; } = string.Empty;
        public string CoachEmail { get; set; } = string.Empty;
        public int AthleteId { get; set; }
        public string AthleteName { get; set; } = string.Empty;
        public string AthleteEmail { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? InvitationMessage { get; set; }
        public string InvitedAt { get; set; } = string.Empty;
        public string? RespondedAt { get; set; }
        public string? LinkedSince { get; set; }
    }

    /// <summary>
    /// DTO de respuesta para un coach
    /// </summary>
    public class CoachResponseDto
    {
        public int Id { get; set; }
        public int RelationshipId { get; set; } // ✅ Importante: ID de la relación para poder eliminar
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Status { get; set; } = string.Empty;
        public string LinkedSince { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO de respuesta para un atleta
    /// </summary>
    public class AthleteResponseDto
    {
        public int Id { get; set; }
        public int RelationshipId { get; set; } // ✅ Importante: ID de la relación para poder eliminar
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Status { get; set; } = string.Empty;
        public string LinkedSince { get; set; } = string.Empty;
        public string? LastActivity { get; set; }
        public int? DaysSinceLastWorkout { get; set; } // Días desde el último entrenamiento completado
        public string? TrainingStartDate { get; set; } // Fecha de inicio de entrenamiento (formato: YYYY-MM)
        public string? VO2Max { get; set; } // Velocidad máxima por km en formato mm:ss (ejemplo: "03:30")
        public DateTime? BirthDate { get; set; } // Fecha de nacimiento completa
        public double? Height { get; set; }
        public double? Weight { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? EmergencyContactRelationship { get; set; }
        public bool HasHealthInsurance { get; set; }
        public string? HealthInsuranceProvider { get; set; }
        public string? HealthInsuranceMemberNumber { get; set; }
        public DateTime? LastCheckupDate { get; set; }
        public DateTime? MedicalClearanceExpiryDate { get; set; }
        public List<string> MedicalConditions { get; set; } = [];
    }
}
