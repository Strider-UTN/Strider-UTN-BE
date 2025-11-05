using System.ComponentModel.DataAnnotations;

namespace StriderWebApi.Dto.Groups
{
    /// <summary>
    /// DTO para actualizar una sede/grupo de entrenamiento existente
    /// </summary>
    public class UpdateTrainingGroupDto
    {
        [MaxLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
        public string? Name { get; set; }

        [MinLength(1, ErrorMessage = "Debe proporcionar al menos un punto de entrenamiento")]
        public List<string>? TrainingPoints { get; set; }

        [MaxLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        public string? Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El límite de miembros debe ser mayor a 0")]
        public int? MaxMembers { get; set; }

        public bool? IsPublic { get; set; }

        public bool? AllowSelfJoin { get; set; }

        public bool? RequireApproval { get; set; }

        public TrainingGroupNotificationsUpdateDto? Notifications { get; set; }
    }

    /// <summary>
    /// DTO para actualizar la configuración de notificaciones de una sede
    /// </summary>
    public class TrainingGroupNotificationsUpdateDto
    {
        public bool? NewMembers { get; set; }

        public bool? CompletedWorkouts { get; set; }

        public bool? Injuries { get; set; }

        public bool? MissedSessions { get; set; }
    }
}
