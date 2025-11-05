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
        IPeriodRepository periodRepository) : IMesocycleService
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
            int periodId,
            int coachId,
            CancellationToken cancellationToken = default)
        {
            var planning = await planningRepository.GetByIdAsync(planningId, cancellationToken);
            if (planning == null || planning.CoachId != coachId)
                throw new UnauthorizedException("No tienes permisos para crear mesociclos en esta planificación");

            var period = await periodRepository.GetByIdAsync(periodId, cancellationToken);
            if (period == null || period.PlanningId != planningId)
                throw new NotFoundException("Período no encontrado o no pertenece a esta planificación");

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
                    WeekNumber = week,
                    StartDate = weekStartDate,
                    EndDate = weekEndDate,
                    Sessions = 0,
                    Volume = 0,
                    Intensity = MicrocycleIntensity.Medium,
                    Focus = null,
                    MesocycleId = createdMesocycle.Id,
                    PeriodId = periodId
                };

                var createdMicrocycle = await microcycleRepository.CreateAsync(microcycle, cancellationToken);
                microcycles.Add(createdMicrocycle);

                currentDate = currentDate.AddDays(7);
            }

            return MapToMesocycleResponseDto(createdMesocycle, microcycles.Count);
        }

        public async Task<MesocycleResponseDto> UpdateAsync(int id, UpdateMesocycleDto dto, int coachId, CancellationToken cancellationToken = default)
        {
            var mesocycle = await mesocycleRepository.GetByIdAsync(id, cancellationToken);
            if (mesocycle == null)
                throw new NotFoundException("Mesociclo no encontrado");

            if (!await ValidateMesocycleAccessAsync(id, coachId, cancellationToken))
                throw new UnauthorizedException("No tienes permisos para actualizar este mesociclo");

            mesocycle.Name = dto.Name;
            mesocycle.StartDate = dto.StartDate;
            mesocycle.EndDate = dto.EndDate;
            mesocycle.WeeksCount = dto.WeeksCount;
            mesocycle.Objective = dto.Objective;
            mesocycle.Status = dto.Status;
            mesocycle.PeriodId = dto.PeriodId;

            var updatedMesocycle = await mesocycleRepository.UpdateAsync(mesocycle, cancellationToken);
            return MapToMesocycleResponseDto(updatedMesocycle);
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
    }
}
