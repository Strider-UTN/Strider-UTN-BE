using StriderWebApi.Dto.Groups;
using StriderWebApi.Dto.Invitation;

namespace StriderWebApi.Services.Interfaces
{
    /// <summary>
    /// Interfaz del servicio para TrainingGroup
    /// </summary>
    public interface ITrainingGroupService
    {
        /// <summary>
        /// Crea una nueva sede (usa el usuario autenticado del token JWT)
        /// </summary>
        Task<TrainingGroupResponseDto> CreateTrainingGroupAsync(CreateTrainingGroupDto dto);

        /// <summary>
        /// Obtiene todas las sedes del coach actual (usa el usuario autenticado del token JWT)
        /// </summary>
        Task<IEnumerable<TrainingGroupResponseDto>> GetMyTrainingGroupsAsync();

        /// <summary>
        /// Obtiene una sede por su ID (valida que pertenezca al usuario autenticado)
        /// </summary>
        Task<TrainingGroupResponseDto?> GetTrainingGroupByIdAsync(int id);

        /// <summary>
        /// Actualiza una sede existente (valida que pertenezca al usuario autenticado)
        /// </summary>
        Task<TrainingGroupResponseDto> UpdateTrainingGroupAsync(int id, UpdateTrainingGroupDto dto);

        /// <summary>
        /// Elimina una sede (valida que pertenezca al usuario autenticado)
        /// </summary>
        Task DeleteTrainingGroupAsync(int id);

        /// <summary>
        /// Obtiene los miembros de una sede (valida que pertenezca al usuario autenticado)
        /// </summary>
        Task<IEnumerable<TrainingGroupMemberResponseDto>> GetGroupMembersAsync(int id);

        /// <summary>
        /// Obtiene las estadísticas de una sede (valida que pertenezca al usuario autenticado)
        /// </summary>
        Task<TrainingGroupStatsDto> GetGroupStatsAsync(int id);

        /// <summary>
        /// Invita a un atleta a una sede (usa el usuario autenticado del token JWT como coach)
        /// </summary>
        Task<GroupInvitationResponseDto> InviteAthleteToGroupAsync(int groupId, InviteGroupMemberDto dto);

        /// <summary>
        /// Obtiene las invitaciones pendientes del atleta actual (usa el usuario autenticado del token JWT)
        /// </summary>
        Task<IEnumerable<GroupInvitationResponseDto>> GetMyPendingInvitationsAsync();

        /// <summary>
        /// Responde a una invitación de sede (usa el usuario autenticado del token JWT)
        /// </summary>
        Task<GroupInvitationResponseDto> RespondToInvitationAsync(int invitationId, RespondToGroupInvitationDto dto);

        /// <summary>
        /// Obtiene las sedes activas del atleta actual (usa el usuario autenticado del token JWT)
        /// </summary>
        Task<IEnumerable<MyTrainingGroupResponseDto>> GetMyGroupsAsync();

        /// <summary>
        /// El atleta abandona una sede (usa el usuario autenticado del token JWT)
        /// </summary>
        Task LeaveGroupAsync(int groupId);

        /// <summary>
        /// El coach elimina un miembro de una sede (usa el usuario autenticado del token JWT)
        /// </summary>
        Task RemoveMemberFromGroupAsync(int groupId, int memberId);
    }
}
