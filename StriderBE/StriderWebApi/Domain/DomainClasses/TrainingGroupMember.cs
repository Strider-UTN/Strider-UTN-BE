using StriderWebApi.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StriderWebApi.Domain.DomainClasses
{
    /// <summary>
    /// Entidad que representa la relación entre un usuario y una sede
    /// (muchos-a-muchos entre User y TrainingGroup)
    /// </summary>
    [Table("TrainingGroupMembers")]
    public class TrainingGroupMember
    {
        [Key]
        public int Id { get; set; }

        // Relación con la sede
        [Required]
        public int TrainingGroupId { get; set; }

        [ForeignKey(nameof(TrainingGroupId))]
        public TrainingGroup TrainingGroup { get; set; } = null!;

        // Relación con el usuario (atleta o coach)
        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        // Mensaje de invitación que envía el Coach
        public string? InvitationMessage { get; set; }

        // Fecha de ingreso a la sede
        [Required]
        public DateTime JoinedDate { get; set; } = DateTime.UtcNow;

        // Estado del miembro en la sede
        [Required]
        [MaxLength(50)]
        public TrainingGroupMemberStatus Status { get; set; } = TrainingGroupMemberStatus.Pending;
    }
}
