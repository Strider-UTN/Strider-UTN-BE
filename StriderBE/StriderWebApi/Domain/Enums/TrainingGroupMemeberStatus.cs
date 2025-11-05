namespace StriderWebApi.Domain.Enums
{
    /// <summary>
    /// Enum que representa el estado de la relación entre un atleta y un grupo de entrenamiento
    /// </summary>
    public enum TrainingGroupMemberStatus
    {
        /// <summary>
        /// La invitación está pendiente de respuesta
        /// </summary>
        Pending = 0,

        /// <summary>
        /// El atleta ha aceptado y está activo en el grupo
        /// </summary>
        Active = 1,

        /// <summary>
        /// El atleta ha sido desactivado (temporalmente)
        /// </summary>
        Inactive = 2,

        /// <summary>
        /// El atleta ha rechazado la invitación
        /// </summary>
        Rejected = 3,

        /// <summary>
        /// La invitación ha sido cancelada
        /// </summary>
        Cancelled = 4
    }
}
