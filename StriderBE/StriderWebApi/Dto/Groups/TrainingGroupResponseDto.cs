namespace StriderWebApi.Dto.Groups
{
    /// <summary>
    /// DTO de respuesta para una sede/grupo de entrenamiento
    /// </summary>
    public class TrainingGroupResponseDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string CreatedDate { get; set; } = string.Empty; // ISO format (yyyy-MM-dd)

        public int CreatedByUserId { get; set; }

        public string CreatedByName { get; set; } = string.Empty;

        public List<string> TrainingPoints { get; set; } = new();

        public int MemberCount { get; set; }

        public int? MaxMembers { get; set; }

        public bool IsPublic { get; set; }

        public bool AllowSelfJoin { get; set; }

        public bool RequireApproval { get; set; }

        public TrainingGroupNotificationsResponseDto? Notifications { get; set; }
    }

    /// <summary>
    /// DTO para la configuración de notificaciones en la respuesta
    /// </summary>
    public class TrainingGroupNotificationsResponseDto
    {
        public bool NewMembers { get; set; }

        public bool CompletedWorkouts { get; set; }

        public bool Injuries { get; set; }

        public bool MissedSessions { get; set; }
    }
}
