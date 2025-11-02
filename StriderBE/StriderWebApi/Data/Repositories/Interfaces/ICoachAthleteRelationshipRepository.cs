using StriderWebApi.Domain;
using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Data.Repositories.Interfaces
{
    public interface ICoachAthleteRelationshipRepository
    {
        /// <summary>
        /// Crea una nueva relación coach-atleta
        /// </summary>
        Task<CoachAthleteRelationship> CreateRelationshipAsync(CoachAthleteRelationship relationship, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene una relación por ID
        /// </summary>
        Task<CoachAthleteRelationship?> GetRelationshipByIdAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene una relación activa entre un coach y un atleta
        /// </summary>
        Task<CoachAthleteRelationship?> GetRelationshipByCoachAndAthleteAsync(int coachId, int athleteId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene todas las relaciones de un atleta con un status específico
        /// </summary>
        Task<List<CoachAthleteRelationship>> GetRelationshipsByAthleteAsync(int athleteId, CoachAthleteRelationshipStatus? status = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene todas las relaciones de un coach con un status específico
        /// </summary>
        Task<List<CoachAthleteRelationship>> GetRelationshipsByCoachAsync(int coachId, CoachAthleteRelationshipStatus? status = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene las invitaciones pendientes de un atleta
        /// </summary>
        Task<List<CoachAthleteRelationship>> GetPendingInvitationsByAthleteAsync(int athleteId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Actualiza una relación existente
        /// </summary>
        Task<CoachAthleteRelationship> UpdateRelationshipAsync(CoachAthleteRelationship relationship, CancellationToken cancellationToken = default);

        /// <summary>
        /// Elimina una relación
        /// </summary>
        Task<bool> DeleteRelationshipAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica si existe una relación activa entre un coach y un atleta
        /// </summary>
        Task<bool> RelationshipExistsAsync(int coachId, int athleteId, CancellationToken cancellationToken = default);
    }
}
