using StriderWebApi.Data.Repositories;
using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Domain.Enums;
using StriderWebApi.Dto.Mesocycle;
using StriderWebApi.Exceptions;
using StriderWebApi.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace StriderWebApi.Services
{
    public class MesocycleService(
        IMesocycleRepository mesocycleRepository,
        IMicrocycleRepository microcycleRepository,
        IPlanningRepository planningRepository,
        ITrainingSessionsRepository trainingSessionsRepository) : IMesocycleService
    {
        public async Task<MesocycleResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var mesocycle = await mesocycleRepository.GetByIdWithMicrocyclesAsync(id, cancellationToken);
            if (mesocycle == null)
                throw new NotFoundException("Mesociclo no encontrado");

            return MapToMesocycleResponseDto(mesocycle);
        }

        public async Task<IEnumerable<MesocycleResponseDto>> GetByPlanningIdAsync(int planningId, CancellationToken cancellationToken = default)
        {
            var mesocycles = await mesocycleRepository.GetByPlanningIdWithMicrocyclesAsync(planningId, cancellationToken);
            return mesocycles.Select(mesocycle => MapToMesocycleResponseDto(mesocycle));
        }

        public async Task<MesocycleResponseDto> CreateAsync(CreateMesocycleDto dto, int planningId, int coachId, CancellationToken cancellationToken = default)
        {
            var planning = await planningRepository.GetByIdAsync(planningId, cancellationToken);
            if (planning == null || planning.CoachId != coachId)
                throw new UnauthorizedException("No tienes permisos para crear mesociclos en esta planificación");

            var endDate = dto.StartDate.AddDays((dto.WeeksCount * 7) - 1);

            var mesocycle = new Mesocycle
            {
                Name = dto.Name,
                StartDate = dto.StartDate,
                EndDate = endDate,
                WeeksCount = dto.WeeksCount,
                Objective = dto.Objective,
                Status = dto.Status,
                PlanningId = planningId,
                PeriodId = dto.PeriodId
            };

            var createdMesocycle = await mesocycleRepository.CreateAsync(mesocycle, cancellationToken);
            return MapToMesocycleResponseDto(createdMesocycle);
        }

        public async Task<MesocycleResponseDto> CreateWithAutoMicrocyclesAsync(
            CreateMesocycleDto dto,
            int planningId,
            int coachId,
            CancellationToken cancellationToken = default)
        {
            var planning = await planningRepository.GetByIdAsync(planningId, cancellationToken);
            if (planning == null || planning.CoachId != coachId)
                throw new UnauthorizedException("No tienes permisos para crear mesociclos en esta planificación");

            var endDate = dto.StartDate.AddDays((dto.WeeksCount * 7) - 1);
            var hasOverlap = await mesocycleRepository.HasOverlappingDatesAsync(
                planningId, dto.StartDate, endDate, null, cancellationToken);
            if (hasOverlap)
                throw new ValidationException("El mesociclo se superpone con otro mesociclo existente");

            if (dto.WeeksCount <= 0 || dto.WeeksCount > 52)
                throw new ValidationException("El número de semanas debe estar entre 1 y 52");

            var mesocycle = new Mesocycle
            {
                Name = dto.Name,
                StartDate = dto.StartDate,
                EndDate = endDate,
                WeeksCount = dto.WeeksCount,
                Objective = dto.Objective,
                Status = dto.Status,
                PlanningId = planningId,
                PeriodId = dto.PeriodId
            };

            var createdMesocycle = await mesocycleRepository.CreateAsync(mesocycle, cancellationToken);

            // Crear microciclos automáticamente
            var microcycles = new List<Microcycle>();
            var currentDate = dto.StartDate;

            for (int week = 1; week <= dto.WeeksCount; week++)
            {
                var weekStartDate = currentDate;
                var weekEndDate = currentDate.AddDays(6);

                var microcycle = new Microcycle
                {
                    Name = $"Semana {week}",
                    Description = null,
                    WeekNumber = week,
                    StartDate = weekStartDate,
                    EndDate = weekEndDate,
                    Sessions = 0, // Se calculará automáticamente
                    Volume = 0, // Se calculará automáticamente
                    Intensity = MicrocycleIntensity.Medium,
                    Focus = null,
                    MesocycleId = createdMesocycle.Id,
                };

                var createdMicrocycle = await microcycleRepository.CreateAsync(microcycle, cancellationToken);
                microcycles.Add(createdMicrocycle);

                currentDate = currentDate.AddDays(7);
            }

            return MapToMesocycleResponseDto(createdMesocycle, microcycles.Count);
        }

        public async Task<MesocycleResponseDto> UpdateAsync(
             int id,
             UpdateMesocycleDto dto,
             int coachId,
             CancellationToken cancellationToken = default)
        {
            var mesocycle = await mesocycleRepository.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException("Mesociclo no encontrado");
            if (!await ValidateMesocycleAccessAsync(id, coachId, cancellationToken))
                throw new UnauthorizedException("No tienes permisos para actualizar este mesociclo");

            // Obtener microciclos actuales del mesociclo
            var existingMicrocycles = await microcycleRepository.GetByMesocycleIdAsync(id, cancellationToken);

            // Validar que no se puedan eliminar microciclos con sesiones
            if (dto.WeeksCount < mesocycle.WeeksCount)
            {
                var weeksToRemove = mesocycle.WeeksCount - dto.WeeksCount;
                // Obtener los microciclos que se eliminarían (los últimos)
                var microcyclesToRemove = existingMicrocycles
                    .OrderByDescending(m => m.WeekNumber)
                    .Take(weeksToRemove)
                    .ToList();

                // Verificar si alguno de estos microciclos tiene sesiones
                var microcyclesWithSessions = new List<int>();
                foreach (var microcycle in microcyclesToRemove)
                {
                    var sessions = await trainingSessionsRepository.GetByMicrocycleIdAsync(microcycle.Id, cancellationToken);
                    if (sessions.Any())
                    {
                        microcyclesWithSessions.Add(microcycle.WeekNumber);
                    }
                }

                if (microcyclesWithSessions.Any())
                {
                    var minWeeksRequired = mesocycle.WeeksCount - microcyclesWithSessions.Count;
                    throw new ValidationException(
                        $"No se pueden eliminar las semanas {string.Join(", ", microcyclesWithSessions)} " +
                        $"porque contienen sesiones de entrenamiento. " +
                        $"Debe mantener al menos {minWeeksRequired} semanas."
                    );
                }
            }

            // Actualizar propiedades del mesociclo
            mesocycle.Name = dto.Name;
            mesocycle.StartDate = dto.StartDate;
            mesocycle.EndDate = dto.EndDate;
            mesocycle.WeeksCount = dto.WeeksCount;
            mesocycle.Objective = dto.Objective;
            mesocycle.Status = dto.Status;
            mesocycle.PeriodId = dto.PeriodId;

            var updatedMesocycle = await mesocycleRepository.UpdateAsync(mesocycle, cancellationToken);

            // Gestionar microciclos según el cambio en WeeksCount
            if (dto.WeeksCount != existingMicrocycles.Count)
            {
                await SyncMicrocyclesForMesocycleAsync(
                    updatedMesocycle,
                    existingMicrocycles.ToList(),
                    cancellationToken);
            }

            // Contar microciclos actualizados
            var currentMicrocycles = await microcycleRepository.GetByMesocycleIdAsync(id, cancellationToken);
            return MapToMesocycleResponseDto(updatedMesocycle, currentMicrocycles.Count());
        }

        public async Task<bool> DeleteAsync(int id, int coachId, CancellationToken cancellationToken = default)
        {
            if (!await ValidateMesocycleAccessAsync(id, coachId, cancellationToken))
                throw new UnauthorizedException("No tienes permisos para eliminar este mesociclo");

            return await mesocycleRepository.DeleteAsync(id, cancellationToken);
        }

        public async Task<bool> ValidateMesocycleAccessAsync(int mesocycleId, int coachId, CancellationToken cancellationToken = default)
        {
            var mesocycle = await mesocycleRepository.GetByIdAsync(mesocycleId, cancellationToken);
            if (mesocycle == null) return false;

            var planning = await planningRepository.GetByIdAsync(mesocycle.PlanningId, cancellationToken);
            return planning != null && planning.CoachId == coachId;
        }

        public async Task<bool> ValidateNoDateOverlapAsync(int planningId, DateTime startDate, DateTime endDate, int? excludeMesocycleId = null, CancellationToken cancellationToken = default)
        {
            return !await mesocycleRepository.HasOverlappingDatesAsync(planningId, startDate, endDate, excludeMesocycleId, cancellationToken);
        }

        private MesocycleResponseDto MapToMesocycleResponseDto(Mesocycle mesocycle, int? microcyclesCount = null)
        {
            return new MesocycleResponseDto
            {
                Id = mesocycle.Id,
                Name = mesocycle.Name,
                StartDate = mesocycle.StartDate,
                EndDate = mesocycle.EndDate,
                Objective = mesocycle.Objective,
                WeeksCount = mesocycle.WeeksCount,
                Status = mesocycle.Status,
                PlanningId = mesocycle.PlanningId,
                PeriodId = mesocycle.PeriodId,
                MicrocyclesCount = microcyclesCount ?? mesocycle.Microcycles?.Count ?? 0,
                CreatedAt = mesocycle.CreatedAt,
                UpdatedAt = mesocycle.UpdatedAt
            };
        }

        private async Task SyncMicrocyclesForMesocycleAsync(
                Mesocycle mesocycle,
                List<Microcycle> existingMicrocycles,
                CancellationToken cancellationToken)
        {
            var currentCount = existingMicrocycles.Count;
            var targetCount = mesocycle.WeeksCount;

            if (targetCount > currentCount)
            {
                // Agregar microciclos faltantes
                var weeksToAdd = targetCount - currentCount;
                var lastWeekNumber = existingMicrocycles.Any()
                    ? existingMicrocycles.Max(m => m.WeekNumber)
                    : 0;

                // Calcular la fecha de inicio del próximo microciclo
                var lastMicrocycle = existingMicrocycles
                    .OrderByDescending(m => m.WeekNumber)
                    .FirstOrDefault();

                var currentDate = lastMicrocycle != null
                    ? lastMicrocycle.EndDate.AddDays(1)
                    : mesocycle.StartDate;

                for (int week = 1; week <= weeksToAdd; week++)
                {
                    var weekNumber = lastWeekNumber + week;
                    var weekStartDate = currentDate;
                    var weekEndDate = currentDate.AddDays(6);

                    var newMicrocycle = new Microcycle
                    {
                        Name = $"Semana {weekNumber}",
                        Description = null,
                        WeekNumber = weekNumber,
                        StartDate = weekStartDate,
                        EndDate = weekEndDate,
                        Sessions = 0, // Se calculará automáticamente
                        Volume = 0, // Se calculará automáticamente
                        Intensity = MicrocycleIntensity.Medium,
                        Focus = null,
                        MesocycleId = mesocycle.Id
                    };

                    await microcycleRepository.CreateAsync(newMicrocycle, cancellationToken);
                    currentDate = currentDate.AddDays(7);
                }
            }
            else if (targetCount < currentCount)
            {
                // Eliminar microciclos sobrantes (los últimos)
                // NOTA: Ya validamos que no tengan sesiones en UpdateAsync
                var weeksToRemove = currentCount - targetCount;
                var microcyclesToRemove = existingMicrocycles
                    .OrderByDescending(m => m.WeekNumber)
                    .Take(weeksToRemove)
                    .ToList();

                foreach (var microcycle in microcyclesToRemove)
                {
                    await microcycleRepository.DeleteAsync(microcycle.Id, cancellationToken);
                }
            }
        }
    }
}
