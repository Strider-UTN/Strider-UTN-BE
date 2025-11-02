namespace StriderWebApi.Domain.Enums
{
    public enum CoachAthleteRelationshipStatus
    {
        Pending = 1,    // Invitación pendiente de respuesta
        Accepted = 2,   // Aceptada por el atleta
        Rejected = 3,   // Rechazada por el atleta
        Inactive = 4,   // Desactivada (por coach o atleta)
        Cancelled = 5   // Cancelada antes de responder
    }
}
