using StriderWebApi.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace StriderWebApi.Domain.DomainClasses
{
    /// <summary>
    /// Tabla de unión entre TrainingSession y Athlete/User
    /// Permite múltiples atletas por sesión y estado individual por atleta
    /// </summary>
    [Table("TrainingSessionAthletes")]
    public class TrainingSessionAthlete
    {
        public int Id { get; set; }

        public int TrainingSessionId { get; set; }
        public TrainingSession TrainingSession { get; set; } = null!;

        public int AthleteId { get; set; }
        public User Athlete { get; set; } = null!;

        // Estado de la sesión para este atleta específico
        public SessionStatus Status { get; set; } = SessionStatus.Pending;
        public DateTime? CompletedAt { get; set; }

        // Datos reales del entrenamiento (cuando el atleta completa la sesión)
        public double? ActualDistance { get; set; } // en metros
        public int? ActualDuration { get; set; } // en minutos
        public string? ActualAvgPace { get; set; } // formato "mm:ss"
        public int? ActualAvgHR { get; set; }
        public int? ActualMaxHR { get; set; }

        // Metadata
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }
}
