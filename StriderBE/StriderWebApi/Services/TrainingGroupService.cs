using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Domain.Enums;
using StriderWebApi.Dto.Groups;
using StriderWebApi.Dto.Invitation;
using StriderWebApi.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace StriderWebApi.Services
{
    /// <summary>
    /// Implementación del servicio para TrainingGroup
    /// </summary>
    public class TrainingGroupService(
        ITrainingGroupRepository repository,
        IUserRepository userRepository,
        IJwtService jwtService) : ITrainingGroupService
    {
        public async Task<TrainingGroupResponseDto> CreateTrainingGroupAsync(CreateTrainingGroupDto dto)
        {
            var coachId = jwtService.GetCurrentUserId();
            if (coachId == null)
            {
                throw new UnauthorizedAccessException("Token inválido o usuario no autenticado");
            }

            // Validar que el usuario es un coach
            var coach = await userRepository.GetUserByIdAsync(coachId.Value);
            if (coach == null)
            {
                throw new KeyNotFoundException($"Usuario con ID {coachId} no encontrado");
            }

            if (coach.UserType != UserTypeEnum.Coach)
            {
                throw new UnauthorizedAccessException("Solo los entrenadores pueden crear sedes");
            }

            // Crear la entidad
            var trainingGroup = new TrainingGroup
            {
                Name = dto.Name,
                Description = dto.Description,
                CreatedByUserId = coachId.Value,
                CreatedDate = DateTime.UtcNow,
                MaxMembers = dto.MaxMembers,
                IsPublic = dto.IsPublic,
                AllowSelfJoin = dto.AllowSelfJoin,
                RequireApproval = dto.RequireApproval,
                TrainingPoints = dto.TrainingPoints.Select(tp => new TrainingPoint
                {
                    Name = tp,
                    CreatedAt = DateTime.UtcNow
                }).ToList(),
                Notifications = new TrainingGroupNotifications
                {
                    NewMembers = true,
                    CompletedWorkouts = true,
                    Injuries = true,
                    MissedSessions = true
                }
            };

            // Guardar en la base de datos
            var createdGroup = await repository.CreateAsync(trainingGroup);

            // Cargar la relación con CreatedBy para el mapeo
            createdGroup = await repository.GetByIdAsync(createdGroup.Id);

            // Mapear a DTO de respuesta
            return MapToResponseDto(createdGroup!);
        }

        public async Task<IEnumerable<TrainingGroupResponseDto>> GetMyTrainingGroupsAsync()
        {
            var coachId = jwtService.GetCurrentUserId();
            if (coachId == null)
            {
                throw new UnauthorizedAccessException("Token inválido o usuario no autenticado");
            }

            var groups = await repository.GetByCoachIdAsync(coachId.Value);
            return groups.Select(MapToResponseDto);
        }

        public async Task<TrainingGroupResponseDto?> GetTrainingGroupByIdAsync(int id)
        {
            var coachId = jwtService.GetCurrentUserId();
            if (coachId == null)
            {
                throw new UnauthorizedAccessException("Token inválido o usuario no autenticado");
            }

            var group = await repository.GetByIdAsync(id);
            if (group == null)
            {
                return null;
            }

            // Verificar que el coach es el dueño de la sede
            if (group.CreatedByUserId != coachId.Value)
            {
                throw new UnauthorizedAccessException("No tienes permiso para ver esta sede");
            }

            return MapToResponseDto(group);
        }

        public async Task<TrainingGroupResponseDto> UpdateTrainingGroupAsync(int id, UpdateTrainingGroupDto dto)
        {
            var coachId = jwtService.GetCurrentUserId();
            if (coachId == null)
            {
                throw new UnauthorizedAccessException("Token inválido o usuario no autenticado");
            }

            var group = await repository.GetByIdAsync(id);
            if (group == null)
            {
                throw new KeyNotFoundException($"Sede con ID {id} no encontrada");
            }

            // Verificar que el coach es el dueño de la sede
            if (group.CreatedByUserId != coachId.Value)
            {
                throw new UnauthorizedAccessException("No tienes permiso para modificar esta sede");
            }

            // Actualizar propiedades
            if (dto.Name != null) group.Name = dto.Name;
            if (dto.Description != null) group.Description = dto.Description;
            if (dto.MaxMembers != null) group.MaxMembers = dto.MaxMembers;
            if (dto.IsPublic != null) group.IsPublic = dto.IsPublic.Value;
            if (dto.AllowSelfJoin != null) group.AllowSelfJoin = dto.AllowSelfJoin.Value;
            if (dto.RequireApproval != null) group.RequireApproval = dto.RequireApproval.Value;

            // Actualizar puntos de entrenamiento si se proporcionan
            if (dto.TrainingPoints != null && dto.TrainingPoints.Any())
            {
                // Eliminar puntos existentes
                // EF Core manejará la eliminación en cascada automáticamente
                group.TrainingPoints.Clear();

                // Agregar nuevos puntos
                foreach (var tp in dto.TrainingPoints)
                {
                    group.TrainingPoints.Add(new TrainingPoint
                    {
                        Name = tp,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            // Actualizar notificaciones si se proporcionan
            if (dto.Notifications != null)
            {
                if (group.Notifications == null)
                {
                    group.Notifications = new TrainingGroupNotifications();
                }
                if (dto.Notifications.NewMembers != null)
                    group.Notifications.NewMembers = dto.Notifications.NewMembers.Value;
                if (dto.Notifications.CompletedWorkouts != null)
                    group.Notifications.CompletedWorkouts = dto.Notifications.CompletedWorkouts.Value;
                if (dto.Notifications.Injuries != null)
                    group.Notifications.Injuries = dto.Notifications.Injuries.Value;
                if (dto.Notifications.MissedSessions != null)
                    group.Notifications.MissedSessions = dto.Notifications.MissedSessions.Value;
            }

            var updatedGroup = await repository.UpdateAsync(group);

            // Recargar para incluir relaciones actualizadas (especialmente TrainingPoints)
            updatedGroup = await repository.GetByIdAsync(updatedGroup.Id);

            return MapToResponseDto(updatedGroup!);
        }

        public async Task DeleteTrainingGroupAsync(int id)
        {
            var coachId = jwtService.GetCurrentUserId();
            if (coachId == null)
            {
                throw new UnauthorizedAccessException("Token inválido o usuario no autenticado");
            }

            var group = await repository.GetByIdAsync(id);
            if (group == null)
            {
                throw new KeyNotFoundException($"Sede con ID {id} no encontrada");
            }

            // Verificar que el coach es el dueño de la sede
            if (group.CreatedByUserId != coachId.Value)
            {
                throw new UnauthorizedAccessException("No tienes permiso para eliminar esta sede");
            }

            await repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<TrainingGroupMemberResponseDto>> GetGroupMembersAsync(int id)
        {
            var coachId = jwtService.GetCurrentUserId();
            if (coachId == null)
            {
                throw new UnauthorizedAccessException("Token inválido o usuario no autenticado");
            }

            var group = await repository.GetByIdAsync(id);
            if (group == null)
            {
                throw new KeyNotFoundException($"Sede con ID {id} no encontrada");
            }

            // Verificar que el coach es el dueño de la sede
            if (group.CreatedByUserId != coachId.Value)
            {
                throw new UnauthorizedAccessException("No tienes permiso para ver los miembros de esta sede");
            }

            var members = await repository.GetMembersByGroupIdAsync(id);
            return members.Select(MapMemberToResponseDto);
        }

        public async Task<TrainingGroupStatsDto> GetGroupStatsAsync(int id)
        {
            var coachId = jwtService.GetCurrentUserId();
            if (coachId == null)
            {
                throw new UnauthorizedAccessException("Token inválido o usuario no autenticado");
            }

            var group = await repository.GetByIdAsync(id);
            if (group == null)
            {
                throw new KeyNotFoundException($"Sede con ID {id} no encontrada");
            }

            // Verificar que el coach es el dueño de la sede
            if (group.CreatedByUserId != coachId.Value)
            {
                throw new UnauthorizedAccessException("No tienes permiso para ver las estadísticas de esta sede");
            }

            var activeMembers = await repository.CountActiveMembersAsync(id);

            // TODO: Implementar lógica para obtener estadísticas de entrenamientos
            // Por ahora retornamos valores por defecto
            return new TrainingGroupStatsDto
            {
                TotalWorkouts = 0,
                CompletedWorkouts = 0,
                PlannedWorkouts = 0,
                ActiveMembers = activeMembers
            };
        }

        public async Task<GroupInvitationResponseDto> InviteAthleteToGroupAsync(int groupId, InviteGroupMemberDto dto)
        {
            var coachId = jwtService.GetCurrentUserId();
            if (coachId == null)
            {
                throw new UnauthorizedAccessException("Token inválido o usuario no autenticado");
            }

            // Validar que el usuario es un coach
            var coach = await userRepository.GetUserByIdAsync(coachId.Value);
            if (coach == null || coach.UserType != UserTypeEnum.Coach)
            {
                throw new UnauthorizedAccessException("Solo los entrenadores pueden invitar atletas a sedes");
            }

            // Validar que la sede existe y pertenece al coach
            var group = await repository.GetByIdAsync(groupId);
            if (group == null)
            {
                throw new KeyNotFoundException($"Sede con ID {groupId} no encontrada");
            }

            if (group.CreatedByUserId != coachId.Value)
            {
                throw new UnauthorizedAccessException("No tienes permiso para invitar atletas a esta sede");
            }

            // Validar que el atleta existe y es un atleta
            var athlete = await userRepository.GetUserByIdAsync(dto.AthleteId);
            if (athlete == null)
            {
                throw new KeyNotFoundException($"Atleta con ID {dto.AthleteId} no encontrado");
            }

            if (athlete.UserType != UserTypeEnum.Athlete)
            {
                throw new ValidationException("El usuario especificado no es un atleta");
            }

            // Verificar que no existe ya una relación (de cualquier estado)
            var existingRelationship = await repository.ExistsMemberRelationshipAsync(groupId, dto.AthleteId);
            if (existingRelationship)
            {
                // Si existe, obtenerla y actualizar el estado a Pending si estaba rechazada/cancelada
                var existingMember = await repository.GetMemberByGroupAndUserAsync(groupId, dto.AthleteId);
                if (existingMember != null)
                {
                    if (existingMember.Status == TrainingGroupMemberStatus.Active)
                    {
                        throw new ValidationException("El atleta ya es miembro activo de esta sede");
                    }

                    // Reactivar la invitación si estaba rechazada o cancelada
                    if (existingMember.Status == TrainingGroupMemberStatus.Rejected ||
                        existingMember.Status == TrainingGroupMemberStatus.Cancelled)
                    {
                        existingMember.Status = TrainingGroupMemberStatus.Pending;
                        existingMember.JoinedDate = DateTime.UtcNow;
                        existingMember.InvitationMessage = dto.Message;
                        var updated = await repository.UpdateMemberInvitationStatusAsync(existingMember.Id, TrainingGroupMemberStatus.Pending);
                        return MapToInvitationResponseDto(updated!);
                    }
                }

                throw new ValidationException("Ya existe una relación entre este atleta y la sede");
            }

            // Verificar límite de miembros si existe
            if (group.MaxMembers.HasValue)
            {
                var activeMembers = await repository.CountActiveMembersAsync(groupId);
                if (activeMembers >= group.MaxMembers.Value)
                {
                    throw new ValidationException($"La sede ha alcanzado su límite máximo de {group.MaxMembers.Value} miembros");
                }
            }

            // Crear la invitación
            var member = new TrainingGroupMember
            {
                TrainingGroupId = groupId,
                UserId = dto.AthleteId,
                Status = TrainingGroupMemberStatus.Pending,
                JoinedDate = DateTime.UtcNow,
                InvitationMessage = dto.Message
            };

            var created = await repository.CreateMemberInvitationAsync(member);
            return MapToInvitationResponseDto(created!);
        }

        public async Task<IEnumerable<GroupInvitationResponseDto>> GetMyPendingInvitationsAsync()
        {
            var athleteId = jwtService.GetCurrentUserId();
            if (athleteId == null)
            {
                throw new UnauthorizedAccessException("Token inválido o usuario no autenticado");
            }

            var invitations = await repository.GetPendingInvitationsByAthleteIdAsync(athleteId.Value);
            return invitations.Select(MapToInvitationResponseDto);
        }

        public async Task<GroupInvitationResponseDto> RespondToInvitationAsync(int invitationId, RespondToGroupInvitationDto dto)
        {
            var athleteId = jwtService.GetCurrentUserId();
            if (athleteId == null)
            {
                throw new UnauthorizedAccessException("Token inválido o usuario no autenticado");
            }

            var invitation = await repository.GetMemberInvitationByIdAsync(invitationId);
            if (invitation == null)
            {
                throw new KeyNotFoundException($"Invitación con ID {invitationId} no encontrada");
            }

            // Verificar que la invitación pertenece al atleta autenticado
            if (invitation.UserId != athleteId.Value)
            {
                throw new UnauthorizedAccessException("No tienes permiso para responder a esta invitación");
            }

            // Verificar que la invitación está pendiente
            if (invitation.Status != TrainingGroupMemberStatus.Pending)
            {
                throw new ValidationException("Esta invitación ya ha sido respondida");
            }

            var newStatus = dto.Accept
                ? TrainingGroupMemberStatus.Active
                : TrainingGroupMemberStatus.Rejected;

            var updated = await repository.UpdateMemberInvitationStatusAsync(invitationId, newStatus);
            return MapToInvitationResponseDto(updated!);
        }

        public async Task<IEnumerable<MyTrainingGroupResponseDto>> GetMyGroupsAsync()
        {
            var athleteId = jwtService.GetCurrentUserId();
            if (athleteId == null)
            {
                throw new UnauthorizedAccessException("Token inválido o usuario no autenticado");
            }

            var members = await repository.GetActiveGroupsByAthleteIdAsync(athleteId.Value);
            return members.Select(MapToGroupResponseDto);
        }

        public async Task LeaveGroupAsync(int groupId)
        {
            var athleteId = jwtService.GetCurrentUserId();
            if (athleteId == null)
            {
                throw new UnauthorizedAccessException("Token inválido o usuario no autenticado");
            }

            var member = await repository.GetMemberByGroupAndUserAsync(groupId, athleteId.Value);
            if (member == null)
            {
                throw new KeyNotFoundException("No eres miembro de esta sede");
            }

            if (member.Status != TrainingGroupMemberStatus.Active)
            {
                throw new ValidationException("Solo puedes abandonar sedes donde eres miembro activo");
            }

            var removed = await repository.RemoveMemberAsync(member.Id);
            if (!removed)
            {
                throw new InvalidOperationException("Error al abandonar la sede");
            }
        }

        public async Task RemoveMemberFromGroupAsync(int groupId, int memberId)
        {
            var coachId = jwtService.GetCurrentUserId();
            if (coachId == null)
            {
                throw new UnauthorizedAccessException("Token inválido o usuario no autenticado");
            }

            // Validar que el usuario es un coach
            var coach = await userRepository.GetUserByIdAsync(coachId.Value);
            if (coach == null || coach.UserType != UserTypeEnum.Coach)
            {
                throw new UnauthorizedAccessException("Solo los entrenadores pueden eliminar miembros de sedes");
            }

            // Validar que la sede existe y pertenece al coach
            var group = await repository.GetByIdAsync(groupId);
            if (group == null)
            {
                throw new KeyNotFoundException($"Sede con ID {groupId} no encontrada");
            }

            if (group.CreatedByUserId != coachId.Value)
            {
                throw new UnauthorizedAccessException("No tienes permiso para eliminar miembros de esta sede");
            }

            // Verificar que el miembro existe y pertenece a la sede
            var member = await repository.GetMemberInvitationByIdAsync(memberId);
            if (member == null)
            {
                throw new KeyNotFoundException($"Miembro con ID {memberId} no encontrado");
            }

            if (member.TrainingGroupId != groupId)
            {
                throw new ValidationException("El miembro no pertenece a esta sede");
            }

            var removed = await repository.RemoveMemberAsync(memberId);
            if (!removed)
            {
                throw new InvalidOperationException("Error al eliminar el miembro");
            }
        }

        // ===== MÉTODOS PRIVADOS DE MAPEO =====

        private GroupInvitationResponseDto MapToInvitationResponseDto(TrainingGroupMember member)
        {
            return new GroupInvitationResponseDto
            {
                Id = member.Id,
                TrainingGroupId = member.TrainingGroupId,
                TrainingGroupName = member.TrainingGroup?.Name ?? "Desconocido",
                CoachId = member.TrainingGroup?.CreatedByUserId ?? 0,
                CoachName = member.TrainingGroup?.CreatedBy?.FullName ?? "Desconocido",
                CoachEmail = member.TrainingGroup?.CreatedBy?.Email ?? "Desconocido",
                AthleteId = member.UserId,
                AthleteName = member.User?.FullName ?? "Desconocido",
                AthleteEmail = member.User?.Email ?? "Desconocido",
                Status = member.Status,
                InvitationMessage = member.InvitationMessage, // Si tienes este campo
                JoinedDate = member.JoinedDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                RespondedAt = member.Status != TrainingGroupMemberStatus.Pending
                    ? DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                    : null
            };
        }

        private MyTrainingGroupResponseDto MapToGroupResponseDto(TrainingGroupMember member)
        {
            var group = member.TrainingGroup;
            return new MyTrainingGroupResponseDto
            {
                Id = member.Id,
                TrainingGroupId = group?.Id ?? 0,
                TrainingGroupName = group?.Name ?? "Desconocido",
                Description = group?.Description,
                CoachId = group?.CreatedByUserId ?? 0,
                CoachName = group?.CreatedBy?.FullName ?? "Desconocido",
                CoachEmail = group?.CreatedBy?.Email ?? "Desconocido",
                TrainingPoints = group?.TrainingPoints?.Select(tp => tp.Name).ToList() ?? new List<string>(),
                MemberCount = group?.Members?.Count(m => m.Status == TrainingGroupMemberStatus.Active) ?? 0,
                JoinedDate = member.JoinedDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            };
        }

        private TrainingGroupResponseDto MapToResponseDto(TrainingGroup group)
        {
            var memberCount = group.Members?.Count(m => m.Status == TrainingGroupMemberStatus.Active) ?? 0;

            return new TrainingGroupResponseDto
            {
                Id = group.Id,
                Name = group.Name,
                Description = group.Description,
                CreatedDate = group.CreatedDate.ToString("yyyy-MM-dd"),
                CreatedByUserId = group.CreatedByUserId,
                CreatedByName = group.CreatedBy?.FullName ?? "Unknown",
                TrainingPoints = group.TrainingPoints?.Select(tp => tp.Name).ToList() ?? new List<string>(),
                MemberCount = memberCount,
                MaxMembers = group.MaxMembers,
                IsPublic = group.IsPublic,
                AllowSelfJoin = group.AllowSelfJoin,
                RequireApproval = group.RequireApproval,
                Notifications = group.Notifications != null ? new TrainingGroupNotificationsResponseDto
                {
                    NewMembers = group.Notifications.NewMembers,
                    CompletedWorkouts = group.Notifications.CompletedWorkouts,
                    Injuries = group.Notifications.Injuries,
                    MissedSessions = group.Notifications.MissedSessions
                } : null
            };
        }

        private TrainingGroupMemberResponseDto MapMemberToResponseDto(TrainingGroupMember member)
        {
            return new TrainingGroupMemberResponseDto
            {
                Id = member.Id,
                TrainingGroupId = member.TrainingGroupId,
                TrainingGroupName = member.TrainingGroup?.Name ?? string.Empty,
                UserId = member.UserId,
                Name = member.User?.FullName ?? "Unknown",
                Email = member.User?.Email ?? string.Empty,
                ProfileImage = null, // TODO: Implementar cuando se tenga soporte para almacenamiento de imágenes de perfil
                JoinedDate = member.JoinedDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                Status = member.Status
            };
        }
    }
}
