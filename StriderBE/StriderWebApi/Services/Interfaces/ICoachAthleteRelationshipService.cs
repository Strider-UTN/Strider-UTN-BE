using StriderWebApi.Dto.Invitation;

namespace StriderWebApi.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de relaciones entre coaches y atletas
    /// </summary>
    public interface ICoachAthleteRelationshipService
    {
        /// <summary>
        /// Invita a un atleta por email (solo coaches)
        /// </summary>
        Task<CoachAthleteRelationshipResponseDto> InviteAthleteAsync(InviteAthleteDto dto, CancellationToken cancellationToken = default);

        /// <summary>
        /// Responde a una invitación (aceptar/rechazar)
        /// </summary>
        Task<CoachAthleteRelationshipResponseDto> RespondToInvitationAsync(RespondToInvitationDto dto, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene las invitaciones pendientes del atleta actual
        /// </summary>
        Task<List<CoachAthleteRelationshipResponseDto>> GetPendingInvitationsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene los coaches del atleta actual
        /// </summary>
        Task<List<CoachResponseDto>> GetMyCoachesAsync(string? status = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene los atletas del coach actual
        /// </summary>
        Task<List<AthleteResponseDto>> GetMyAthletesAsync(string? status = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Elimina una relación
        /// </summary>
        Task<bool> RemoveRelationshipAsync(int relationshipId, CancellationToken cancellationToken = default);
    }
}
