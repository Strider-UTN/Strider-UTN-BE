using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StriderWebApi.Domain.DomainClasses
{
    /// <summary>
    /// Entidad que representa una sede o grupo de entrenamiento
    /// </summary>
    [Table("TrainingGroups")]
    public class TrainingGroup
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Relación con el Coach que creó la sede
        [Required]
        public int CreatedByUserId { get; set; }

        [ForeignKey(nameof(CreatedByUserId))]
        public User CreatedBy { get; set; } = null!;

        // Colección de puntos de entrenamiento (ubicaciones)
        public ICollection<TrainingPoint> TrainingPoints { get; set; } = new List<TrainingPoint>();

        // Colección de miembros de la sede
        public ICollection<TrainingGroupMember> Members { get; set; } = new List<TrainingGroupMember>();

        // Configuración de la sede
        public int? MaxMembers { get; set; }

        public bool IsPublic { get; set; } = false;

        public bool AllowSelfJoin { get; set; } = false;

        public bool RequireApproval { get; set; } = true;

        // Configuración de notificaciones (Owned Entity)
        public TrainingGroupNotifications? Notifications { get; set; }
    }
}
