using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Data.Repositories.Interfaces
{
    /// <summary>
    /// Interfaz del repositorio para TrainingGroup
    /// </summary>
    public interface ITrainingGroupRepository
    {
        /// <summary>
        /// Obtiene una sede por su ID
        /// </summary>
        Task<TrainingGroup?> GetByIdAsync(int id);

        /// <summary>
        /// Obtiene todas las sedes creadas por un coach específico
        /// </summary>
        Task<IEnumerable<TrainingGroup>> GetByCoachIdAsync(int coachId);

        /// <summary>
        /// Crea una nueva sede
        /// </summary>
        Task<TrainingGroup> CreateAsync(TrainingGroup trainingGroup);

        /// <summary>
        /// Actualiza una sede existente
        /// </summary>
        Task<TrainingGroup> UpdateAsync(TrainingGroup trainingGroup);

        /// <summary>
        /// Elimina una sede por su ID
        /// </summary>
        Task DeleteAsync(int id);

        /// <summary>
        /// Verifica si una sede existe
        /// </summary>
        Task<bool> ExistsAsync(int id);

        /// <summary>
        /// Obtiene los miembros activos de una sede
        /// </summary>
        Task<IEnumerable<TrainingGroupMember>> GetMembersByGroupIdAsync(int groupId);

        /// <summary>
        /// Cuenta los miembros activos de una sede
        /// </summary>
        Task<int> CountActiveMembersAsync(int groupId);

        /// <summary>
        /// Crea una nueva invitación de miembro a una sede
        /// </summary>
        Task<TrainingGroupMember?> CreateMemberInvitationAsync(TrainingGroupMember member);

        /// <summary>
        /// Obtiene una invitación por su ID
        /// </summary>
        Task<TrainingGroupMember?> GetMemberInvitationByIdAsync(int invitationId);

        /// <summary>
        /// Obtiene las invitaciones pendientes de un atleta
        /// </summary>
        Task<IEnumerable<TrainingGroupMember>> GetPendingInvitationsByAthleteIdAsync(int athleteId);

        /// <summary>
        /// Obtiene las sedes activas de un atleta
        /// </summary>
        Task<IEnumerable<TrainingGroupMember>> GetActiveGroupsByAthleteIdAsync(int athleteId);

        /// <summary>
        /// Verifica si ya existe una relación (de cualquier estado) entre un atleta y una sede
        /// </summary>
        Task<bool> ExistsMemberRelationshipAsync(int trainingGroupId, int athleteId);

        /// <summary>
        /// Actualiza el estado de una invitación
        /// </summary>
        Task<TrainingGroupMember?> UpdateMemberInvitationStatusAsync(int invitationId, TrainingGroupMemberStatus status);

        /// <summary>
        /// Elimina un miembro de una sede (por ID del TrainingGroupMember)
        /// </summary>
        Task<bool> RemoveMemberAsync(int memberId);

        /// <summary>
        /// Obtiene un miembro específico por TrainingGroupId y UserId
        /// </summary>
        Task<TrainingGroupMember?> GetMemberByGroupAndUserAsync(int trainingGroupId, int userId);
    }
}
