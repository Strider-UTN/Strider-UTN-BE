using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Domain.Enums;
using StriderWebApi.Dto.Invitation;
using StriderWebApi.Services.Interfaces;

namespace StriderWebApi.Services
{
    /// <summary>
    /// Servicio para gestionar relaciones entre coaches y atletas
    /// </summary>
    public class CoachAthleteRelationshipService(
        ICoachAthleteRelationshipRepository relationshipRepository,
        IUserRepository userRepository,
        ICompletedWorkoutRepository completedWorkoutRepository,
        IJwtService jwtService,
        ILogger<CoachAthleteRelationshipService> logger) : ICoachAthleteRelationshipService
    {
        public async Task<CoachAthleteRelationshipResponseDto> InviteAthleteAsync(
            InviteAthleteDto dto,
            CancellationToken cancellationToken = default)
        {
            // Obtener el coach actual desde el token JWT
            var currentUserId = jwtService.GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                throw new UnauthorizedAccessException("Usuario no autenticado");
            }

            var coach = await userRepository.GetUserByIdAsync(currentUserId.Value);
            if (coach == null)
            {
                throw new KeyNotFoundException("Coach no encontrado");
            }

            // Verificar que el usuario actual es un coach
            if (coach.UserType != UserTypeEnum.Coach)
            {
                throw new UnauthorizedAccessException("Solo los coaches pueden invitar atletas");
            }

            // Buscar al atleta por email
            var athlete = await userRepository.GetUserByEmailAsync(dto.AthleteEmail, UserTypeEnum.Athlete);
            if (athlete == null)
            {
                throw new KeyNotFoundException($"No se encontró un usuario con el email {dto.AthleteEmail}");
            }

            // Verificar que el usuario encontrado es un atleta
            if (athlete.UserType != UserTypeEnum.Athlete)
            {
                throw new InvalidOperationException($"El usuario con email {dto.AthleteEmail} no es un atleta");
            }

            // Verificar que no existe una relación activa
            var existingRelationship = await relationshipRepository.GetRelationshipByCoachAndAthleteAsync(
                coach.Id,
                athlete.Id,
                cancellationToken);

            if (existingRelationship != null)
            {
                // Si la relación está en estado Pending, devolver error
                if (existingRelationship.Status == CoachAthleteRelationshipStatus.Pending)
                {
                    throw new InvalidOperationException("Ya existe una invitación pendiente para este atleta");
                }

                // Si la relación está en estado Accepted, devolver error
                if (existingRelationship.Status == CoachAthleteRelationshipStatus.Accepted)
                {
                    throw new InvalidOperationException("Ya tienes una relación activa con este atleta");
                }

                // Si la relación está rechazada o cancelada, crear una nueva
                // Actualizar la relación existente a Pending
                existingRelationship.Status = CoachAthleteRelationshipStatus.Pending;
                existingRelationship.InvitationMessage = dto.Message;
                existingRelationship.InvitedAt = DateTime.UtcNow;
                existingRelationship.RespondedAt = null;
                existingRelationship.LinkedSince = null;

                var updatedRelationship = await relationshipRepository.UpdateRelationshipAsync(
                    existingRelationship,
                    cancellationToken);

                return MapToResponseDto(updatedRelationship);
            }

            // Crear nueva relación
            var relationship = new CoachAthleteRelationship
            {
                CoachId = coach.Id,
                AthleteId = athlete.Id,
                Status = CoachAthleteRelationshipStatus.Pending,
                InvitationMessage = dto.Message,
                InvitedAt = DateTime.UtcNow
            };

            var createdRelationship = await relationshipRepository.CreateRelationshipAsync(
                relationship,
                cancellationToken);

            // Cargar la relación con los usuarios incluidos
            var relationshipWithUsers = await relationshipRepository.GetRelationshipByIdAsync(
                createdRelationship.Id,
                cancellationToken);

            if (relationshipWithUsers == null)
            {
                throw new InvalidOperationException("Error al crear la relación");
            }

            // NO ENVIAR EMAIL - El atleta verá la invitación en su panel
            logger.LogInformation(
                "Invitación creada por coach {CoachId} para atleta {AthleteId}",
                coach.Id,
                athlete.Id);

            return MapToResponseDto(relationshipWithUsers);
        }

        public async Task<CoachAthleteRelationshipResponseDto> RespondToInvitationAsync(
            RespondToInvitationDto dto,
            CancellationToken cancellationToken = default)
        {
            // Obtener el atleta actual desde el token JWT
            var currentUserId = jwtService.GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                throw new UnauthorizedAccessException("Usuario no autenticado");
            }

            var athlete = await userRepository.GetUserByIdAsync(currentUserId.Value) ?? throw new KeyNotFoundException("Atleta no encontrado");

            // Verificar que el usuario actual es un atleta
            if (athlete.UserType != UserTypeEnum.Athlete)
            {
                throw new UnauthorizedAccessException("Solo los atletas pueden responder invitaciones");
            }

            // Obtener la relación
            var relationship = await relationshipRepository.GetRelationshipByIdAsync(
                dto.RelationshipId,
                cancellationToken);

            if (relationship == null)
            {
                throw new KeyNotFoundException($"No se encontró la relación con ID {dto.RelationshipId}");
            }

            // Verificar que la relación pertenece al atleta actual
            if (relationship.AthleteId != athlete.Id)
            {
                throw new UnauthorizedAccessException("No tienes permiso para responder esta invitación");
            }

            // Verificar que la relación está en estado Pending
            if (relationship.Status != CoachAthleteRelationshipStatus.Pending)
            {
                throw new InvalidOperationException("Esta invitación ya ha sido respondida");
            }

            // Actualizar el estado de la relación
            relationship.Status = dto.Accept
                ? CoachAthleteRelationshipStatus.Accepted
                : CoachAthleteRelationshipStatus.Rejected;

            relationship.RespondedAt = DateTime.UtcNow;

            if (dto.Accept)
            {
                relationship.LinkedSince = DateTime.UtcNow;
            }

            var updatedRelationship = await relationshipRepository.UpdateRelationshipAsync(
                relationship,
                cancellationToken);

            // Cargar la relación actualizada con los usuarios incluidos
            var relationshipWithUsers = await relationshipRepository.GetRelationshipByIdAsync(
                updatedRelationship.Id,
                cancellationToken);

            if (relationshipWithUsers == null)
            {
                throw new InvalidOperationException("Error al actualizar la relación");
            }

            // NO ENVIAR EMAIL - El coach puede ver en su panel cuando un atleta acepta/rechaza
            logger.LogInformation(
                "Invitación {Status} por atleta {AthleteId} para relación {RelationshipId}",
                dto.Accept ? "aceptada" : "rechazada",
                athlete.Id,
                relationship.Id);

            return MapToResponseDto(relationshipWithUsers);
        }

        public async Task<List<CoachAthleteRelationshipResponseDto>> GetPendingInvitationsAsync(
            CancellationToken cancellationToken = default)
        {
            // Obtener el atleta actual desde el token JWT
            var currentUserId = jwtService.GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                throw new UnauthorizedAccessException("Usuario no autenticado");
            }

            var athlete = await userRepository.GetUserByIdAsync(currentUserId.Value);
            if (athlete == null)
            {
                throw new KeyNotFoundException("Atleta no encontrado");
            }

            // Verificar que el usuario actual es un atleta
            if (athlete.UserType != UserTypeEnum.Athlete)
            {
                throw new UnauthorizedAccessException("Solo los atletas pueden ver invitaciones pendientes");
            }

            var relationships = await relationshipRepository.GetPendingInvitationsByAthleteAsync(
                athlete.Id,
                cancellationToken);

            return relationships.Select(MapToResponseDto).ToList();
        }

        public async Task<List<CoachResponseDto>> GetMyCoachesAsync(
            string? status = null,
            CancellationToken cancellationToken = default)
        {
            // Obtener el atleta actual desde el token JWT
            var currentUserId = jwtService.GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                throw new UnauthorizedAccessException("Usuario no autenticado");
            }

            var athlete = await userRepository.GetUserByIdAsync(currentUserId.Value) ?? throw new KeyNotFoundException("Atleta no encontrado");

            // Verificar que el usuario actual es un atleta
            if (athlete.UserType != UserTypeEnum.Athlete)
            {
                throw new UnauthorizedAccessException("Solo los atletas pueden ver sus coaches");
            }

            CoachAthleteRelationshipStatus? statusEnum = null;
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (Enum.TryParse<CoachAthleteRelationshipStatus>(status, true, out var parsedStatus))
                {
                    statusEnum = parsedStatus;
                }
            }

            // Por defecto, solo devolver coaches aceptados
            if (!statusEnum.HasValue)
            {
                statusEnum = CoachAthleteRelationshipStatus.Accepted;
            }

            var relationships = await relationshipRepository.GetRelationshipsByAthleteAsync(
                athlete.Id,
                statusEnum,
                cancellationToken);

            return relationships.Select(r => new CoachResponseDto
            {
                Id = r.Coach.Id,
                RelationshipId = r.Id,
                Name = r.Coach.FullName ?? r.Coach.Username,
                Email = r.Coach.Email,
                Phone = r.Coach.PhoneNumber,
                Status = r.Status.ToString(),
                LinkedSince = r.LinkedSince?.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? ""
            }).ToList();
        }

        public async Task<List<AthleteResponseDto>> GetMyAthletesAsync(
            string? status = null,
            CancellationToken cancellationToken = default)
        {
            // Obtener el coach actual desde el token JWT
            var currentUserId = jwtService.GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                throw new UnauthorizedAccessException("Usuario no autenticado");
            }

            var coach = await userRepository.GetUserByIdAsync(currentUserId.Value) ?? throw new KeyNotFoundException("Coach no encontrado");

            // Verificar que el usuario actual es un coach
            if (coach.UserType != UserTypeEnum.Coach)
            {
                throw new UnauthorizedAccessException("Solo los coaches pueden ver sus atletas");
            }

            CoachAthleteRelationshipStatus? statusEnum = null;
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (Enum.TryParse<CoachAthleteRelationshipStatus>(status, true, out var parsedStatus))
                {
                    statusEnum = parsedStatus;
                }
            }

            // Por defecto, solo devolver atletas aceptados
            if (!statusEnum.HasValue)
            {
                statusEnum = CoachAthleteRelationshipStatus.Accepted;
            }

            var relationships = await relationshipRepository.GetRelationshipsByCoachAsync(
                coach.Id,
                statusEnum,
                cancellationToken);

            // Calcular último workout por atleta
            var today = DateTime.UtcNow.Date;
            var activityByAthlete = new Dictionary<int, (DateTime? last, int? days)>();
            foreach (var r in relationships)
            {
                var workouts = await completedWorkoutRepository.GetByAthleteIdAsync(r.Athlete.Id, cancellationToken);
                var last = workouts.OrderByDescending(w => w.Date).FirstOrDefault();
                if (last != null)
                {
                    var lastDate = last.Date.Date;
                    activityByAthlete[r.Athlete.Id] = (lastDate, (today - lastDate).Days);
                }
                else
                {
                    activityByAthlete[r.Athlete.Id] = (null, null);
                }
            }

            return relationships.Select(r =>
            {
                var athlete = r.Athlete as Athlete;
                var trainingStartDate = athlete?.TrainingStartDate?.ToString("yyyy-MM") ?? null;

                activityByAthlete.TryGetValue(r.Athlete.Id, out var act);
                var lastIso = act.last.HasValue ? act.last.Value.ToString("yyyy-MM-ddTHH:mm:ssZ") : null;

                return new AthleteResponseDto
                {
                    Id = r.Athlete.Id,
                    RelationshipId = r.Id, // ✅ Importante: incluir el ID de la relación
                    Name = r.Athlete.FullName ?? r.Athlete.Username,
                    Email = r.Athlete.Email,
                    Phone = r.Athlete.PhoneNumber,
                    Status = r.Status.ToString(),
                    LinkedSince = r.LinkedSince?.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? "",
                    LastActivity = lastIso,
                    DaysSinceLastWorkout = act.days,
                    TrainingStartDate = trainingStartDate,
                    VO2Max = athlete?.VO2Max,
                    BirthDate = r.Athlete.BirthDate
                };
            }).ToList();
        }

        public async Task<bool> RemoveRelationshipAsync(int relationshipId, CancellationToken cancellationToken = default)
        {
            // Obtener el usuario actual desde el token JWT
            var currentUserId = jwtService.GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                throw new UnauthorizedAccessException("Usuario no autenticado");
            }

            var currentUser = await userRepository.GetUserByIdAsync(currentUserId.Value);
            if (currentUser == null)
            {
                throw new KeyNotFoundException("Usuario no encontrado");
            }

            // Obtener la relación
            var relationship = await relationshipRepository.GetRelationshipByIdAsync(
                relationshipId,
                cancellationToken);

            if (relationship == null)
            {
                throw new KeyNotFoundException($"No se encontró la relación con ID {relationshipId}");
            }

            // Verificar que el usuario actual es coach o atleta de la relación
            if (relationship.CoachId != currentUser.Id && relationship.AthleteId != currentUser.Id)
            {
                throw new UnauthorizedAccessException("No tienes permiso para eliminar esta relación");
            }

            // Solo permitir eliminar si está aceptada o rechazada (no pendientes)
            if (relationship.Status == CoachAthleteRelationshipStatus.Pending)
            {
                throw new InvalidOperationException("No se puede eliminar una invitación pendiente. Debe ser aceptada o rechazada primero.");
            }

            return await relationshipRepository.DeleteRelationshipAsync(relationshipId, cancellationToken);
        }

        /// <summary>
        /// Mapea una relación a su DTO de respuesta
        /// </summary>
        private CoachAthleteRelationshipResponseDto MapToResponseDto(CoachAthleteRelationship relationship)
        {
            return new CoachAthleteRelationshipResponseDto
            {
                Id = relationship.Id,
                CoachId = relationship.CoachId,
                CoachName = relationship.Coach.FullName ?? relationship.Coach.Username,
                CoachEmail = relationship.Coach.Email,
                AthleteId = relationship.AthleteId,
                AthleteName = relationship.Athlete.FullName ?? relationship.Athlete.Username,
                AthleteEmail = relationship.Athlete.Email,
                Status = relationship.Status.ToString(),
                InvitationMessage = relationship.InvitationMessage,
                InvitedAt = relationship.InvitedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                RespondedAt = relationship.RespondedAt?.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                LinkedSince = relationship.LinkedSince?.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };
        }
    }
}
