using Microsoft.EntityFrameworkCore;

namespace StriderWebApi.Domain.DomainClasses
{
    /// <summary>
    /// Clase Owned Entity que representa la configuración de notificaciones
    /// para una sede. Se almacena como parte de TrainingGroup.
    /// </summary>
    [Owned]
    public class TrainingGroupNotifications
    {
        public bool NewMembers { get; set; } = true;

        public bool CompletedWorkouts { get; set; } = true;

        public bool Injuries { get; set; } = true;

        public bool MissedSessions { get; set; } = true;
    }
}
