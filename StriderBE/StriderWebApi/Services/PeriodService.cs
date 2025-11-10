using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Dto.Period;
using StriderWebApi.Exceptions;
using StriderWebApi.Services.Interfaces;

namespace StriderWebApi.Services
{
    public class PeriodService(
        IPeriodRepository periodRepository,
        IPlanningRepository planningRepository) : IPeriodService
    {
        public async Task<PeriodResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var period = await periodRepository.GetByIdAsync(id, cancellationToken);
            if (period == null)
                throw new NotFoundException("Período no encontrado");

            return MapToPeriodResponseDto(period);
        }

        public async Task<IEnumerable<PeriodResponseDto>> GetByPlanningIdAsync(int planningId, CancellationToken cancellationToken = default)
        {
            var periods = await periodRepository.GetByPlanningIdAsync(planningId, cancellationToken);
            return periods.Select(MapToPeriodResponseDto);
        }

        public async Task<PeriodResponseDto> CreateAsync(CreatePeriodDto dto, int planningId, int coachId, CancellationToken cancellationToken = default)
        {
            var planning = await planningRepository.GetByIdAsync(planningId, cancellationToken);
            if (planning == null || planning.CoachId != coachId)
                throw new UnauthorizedException("No tienes permisos para crear períodos en esta planificación");

            var period = new Period
            {
                Name = dto.Name,
                StartWeek = dto.StartWeek,
                EndWeek = dto.EndWeek,
                Objective = dto.Objective,
                Status = dto.Status,
                PlanningId = planningId
            };

            var createdPeriod = await periodRepository.CreateAsync(period, cancellationToken);
            return MapToPeriodResponseDto(createdPeriod);
        }

        public async Task<PeriodResponseDto> UpdateAsync(int id, UpdatePeriodDto dto, int coachId, CancellationToken cancellationToken = default)
        {
            var period = await periodRepository.GetByIdAsync(id, cancellationToken);
            if (period == null)
                throw new NotFoundException("Período no encontrado");

            if (!await ValidatePeriodAccessAsync(id, coachId, cancellationToken))
                throw new UnauthorizedException("No tienes permisos para actualizar este período");

            period.Name = dto.Name;
            period.StartWeek = dto.StartWeek;
            period.EndWeek = dto.EndWeek;
            period.Objective = dto.Objective;
            period.Status = dto.Status;

            var updatedPeriod = await periodRepository.UpdateAsync(period, cancellationToken);
            return MapToPeriodResponseDto(updatedPeriod);
        }

        public async Task<bool> DeleteAsync(int id, int coachId, CancellationToken cancellationToken = default)
        {
            if (!await ValidatePeriodAccessAsync(id, coachId, cancellationToken))
                throw new UnauthorizedException("No tienes permisos para eliminar este período");

            return await periodRepository.DeleteAsync(id, cancellationToken);
        }

        public async Task<bool> ValidatePeriodAccessAsync(int periodId, int coachId, CancellationToken cancellationToken = default)
        {
            var period = await periodRepository.GetByIdAsync(periodId, cancellationToken);
            if (period == null || !period.PlanningId.HasValue) return false;

            var planning = await planningRepository.GetByIdAsync(period.PlanningId.Value, cancellationToken);
            return planning != null && planning.CoachId == coachId;
        }

        private PeriodResponseDto MapToPeriodResponseDto(Period period)
        {
            return new PeriodResponseDto
            {
                Id = period.Id,
                Name = period.Name,
                StartWeek = period.StartWeek,
                EndWeek = period.EndWeek,
                Objective = period.Objective,
                Status = period.Status,
                PlanningId = period.PlanningId,
                MesocyclesCount = period.Mesocycles?.Count ?? 0,
                MicrocyclesCount = period.Mesocycles?.SelectMany(m => m.Microcycles)?.ToList().Count ?? 0,
                CreatedAt = period.CreatedAt,
                UpdatedAt = period.UpdatedAt
            };
        }
    }
}
