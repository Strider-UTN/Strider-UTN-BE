using StriderWebApi.Data.Repositories;
using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Dto.Planning;
using StriderWebApi.Exceptions;
using StriderWebApi.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace StriderWebApi.Services
{
    public class PlanningService(
        IPlanningRepository planningRepository,
        IPlanningAthleteRepository planningAthleteRepository,
        ITrainingGroupService trainingGroupService) : IPlanningService
    {
        public async Task<PlanningResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var planning = await planningRepository.GetByIdWithDetailsAsync(id, cancellationToken);
            if (planning == null)
                throw new NotFoundException("Planificación no encontrada");

            return MapToPlanningResponseDto(planning);
        }

        public async Task<IEnumerable<PlanningResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var plannings = await planningRepository.GetAllAsync(cancellationToken);
            return plannings.Select(MapToPlanningResponseDto);
        }

        public async Task<IEnumerable<PlanningResponseDto>> GetByCoachIdAsync(int coachId, CancellationToken cancellationToken = default)
        {
            var plannings = await planningRepository.GetByCoachIdAsync(coachId, cancellationToken);
            return plannings.Select(MapToPlanningResponseDto);
        }

        public async Task<PlanningResponseDto> CreateAsync(CreatePlanningDto dto, int coachId, CancellationToken cancellationToken = default)
        {
            var planning = new Planning
            {
                Name = dto.Name,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status,
                CoachId = coachId
            };

            var createdPlanning = await planningRepository.CreateAsync(planning, cancellationToken);

            // Asignar atletas individuales
            if (dto.AthleteIds != null && dto.AthleteIds.Any())
            {
                await AssignAthletesAsync(createdPlanning.Id, dto.AthleteIds, coachId, cancellationToken);
            }

            // Asignar atletas desde grupos
            if (dto.GroupIds != null && dto.GroupIds.Any())
            {
                foreach (var groupId in dto.GroupIds)
                {
                    await AssignAthletesFromGroupAsync(createdPlanning.Id, groupId, coachId, cancellationToken);
                }
            }

            return MapToPlanningResponseDto(createdPlanning);
        }

        public async Task<PlanningResponseDto> UpdateAsync(int id, UpdatePlanningDto dto, int coachId, CancellationToken cancellationToken = default)
        {
            var planning = await planningRepository.GetByIdAsync(id, cancellationToken);
            if (planning == null)
                throw new NotFoundException("Planificación no encontrada");

            if (planning.CoachId != coachId)
                throw new UnauthorizedException("No tienes permisos para actualizar esta planificación");

            planning.Name = dto.Name;
            planning.Description = dto.Description;
            planning.StartDate = dto.StartDate;
            planning.EndDate = dto.EndDate;
            planning.Status = dto.Status;

            var updatedPlanning = await planningRepository.UpdateAsync(planning, cancellationToken);
            return MapToPlanningResponseDto(updatedPlanning);
        }

        public async Task<bool> DeleteAsync(int id, int coachId, CancellationToken cancellationToken = default)
        {
            var planning = await planningRepository.GetByIdAsync(id, cancellationToken);
            if (planning == null) return false;

            if (planning.CoachId != coachId)
                throw new UnauthorizedException("No tienes permisos para eliminar esta planificación");

            return await planningRepository.DeleteAsync(id, cancellationToken);
        }

        public async Task<bool> AssignAthletesAsync(int planningId, IEnumerable<int> athleteIds, int coachId, CancellationToken cancellationToken = default)
        {
            if (!await ValidatePlanningAccessAsync(planningId, coachId, cancellationToken))
                throw new UnauthorizedException("No tienes permisos para asignar atletas a esta planificación");

            var planningAthletes = new List<PlanningAthlete>();
            foreach (var athleteId in athleteIds)
            {
                if (!await planningAthleteRepository.ExistsAsync(planningId, athleteId, cancellationToken))
                {
                    planningAthletes.Add(new PlanningAthlete
                    {
                        PlanningId = planningId,
                        AthleteId = athleteId
                    });
                }
            }

            if (planningAthletes.Any())
            {
                await planningAthleteRepository.CreateMultipleAsync(planningAthletes, cancellationToken);
            }

            return true;
        }

        public async Task<bool> RemoveAthleteAsync(int planningId, int athleteId, int coachId, CancellationToken cancellationToken = default)
        {
            if (!await ValidatePlanningAccessAsync(planningId, coachId, cancellationToken))
                throw new UnauthorizedException("No tienes permisos para eliminar atletas de esta planificación");

            var planningAthletes = await planningAthleteRepository.GetByPlanningIdAsync(planningId, cancellationToken);
            var planningAthlete = planningAthletes.FirstOrDefault(pa => pa.AthleteId == athleteId);

            if (planningAthlete == null)
                return false;

            return await planningAthleteRepository.DeleteAsync(planningAthlete.Id, cancellationToken);
        }

        public async Task<bool> AssignAthletesFromGroupAsync(int planningId, int groupId, int coachId, CancellationToken cancellationToken = default)
        {
            var planning = await planningRepository.GetByIdAsync(planningId, cancellationToken);
            if (planning == null || planning.CoachId != coachId)
                throw new UnauthorizedException("No tienes permisos para asignar atletas a esta planificación");

            var groupMembers = await trainingGroupService.GetGroupMembersAsync(groupId);
            var activeAthletes = groupMembers
                .Where(m => m.Status == Domain.Enums.TrainingGroupMemberStatus.Active)
                .Select(m => m.UserId)
                .ToList();

            if (!activeAthletes.Any())
                throw new ValidationException("El grupo no tiene atletas activos");

            var existingAssignments = await planningAthleteRepository.GetByPlanningIdAsync(planningId, cancellationToken);
            var existingAthleteIds = existingAssignments.Select(a => a.AthleteId).ToHashSet();
            var newAthletes = activeAthletes.Where(id => !existingAthleteIds.Contains(id)).ToList();

            if (!newAthletes.Any())
                throw new ValidationException("Todos los atletas del grupo ya están asignados a esta planificación");

            var planningAthletes = newAthletes.Select(athleteId => new PlanningAthlete
            {
                PlanningId = planningId,
                AthleteId = athleteId
            }).ToList();

            await planningAthleteRepository.CreateMultipleAsync(planningAthletes, cancellationToken);
            return true;
        }

        public async Task<bool> ValidatePlanningAccessAsync(int planningId, int coachId, CancellationToken cancellationToken = default)
        {
            var planning = await planningRepository.GetByIdAsync(planningId, cancellationToken);
            return planning != null && planning.CoachId == coachId;
        }

        public async Task<bool> ValidatePlanningExistsAsync(int planningId, CancellationToken cancellationToken = default)
        {
            return await planningRepository.ExistsAsync(planningId, cancellationToken);
        }

        public async Task<IEnumerable<PlanningResponseDto>> GetByAthleteIdAsync(int athleteId, CancellationToken cancellationToken = default)
        {
            var plannings = await planningRepository.GetByAthleteIdAsync(athleteId, cancellationToken);
            return plannings.Select(MapToPlanningResponseDto);
        }

        public async Task<IEnumerable<PlanningAthleteResponseDto>> GetAssignedAthletesAsync(int planningId, int coachId, CancellationToken cancellationToken = default)
        {
            // Validar que el coach tenga acceso a esta planificación
            var planning = await planningRepository.GetByIdAsync(planningId, cancellationToken);
            if (planning == null)
                throw new NotFoundException("Planificación no encontrada");

            if (planning.CoachId != coachId)
                throw new UnauthorizedException("No tienes permisos para ver los atletas de esta planificación");

            // Obtener los atletas asignados
            var planningAthletes = await planningAthleteRepository.GetByPlanningIdAsync(planningId, cancellationToken);

            // Mapear a DTOs
            return planningAthletes.Select(pa => new PlanningAthleteResponseDto
            {
                Id = pa.Id,
                AthleteId = pa.AthleteId,
                AthleteName = pa.Athlete?.FullName ?? string.Empty,
                AthleteEmail = pa.Athlete?.Email ?? string.Empty,
                PlanningId = pa.PlanningId,
                AssignedAt = pa.CreatedAt
            });
        }

        private PlanningResponseDto MapToPlanningResponseDto(Planning planning)
        {
            return new PlanningResponseDto
            {
                Id = planning.Id,
                Name = planning.Name,
                Description = planning.Description,
                StartDate = planning.StartDate,
                EndDate = planning.EndDate,
                Status = planning.Status,
                CoachId = planning.CoachId,
                CoachName = planning.Coach?.FullName ?? string.Empty,
                AthletesCount = planning.PlanningAthletes?.Count ?? 0,
                MesocyclesCount = planning.Mesocycles?.Count ?? 0,
                PeriodsCount = planning.Periods?.Count ?? 0,
                CreatedAt = planning.CreatedAt,
                UpdatedAt = planning.UpdatedAt
            };
        }
    }
}
