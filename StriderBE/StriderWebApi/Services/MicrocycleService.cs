using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Dto.Microcycle;
using StriderWebApi.Exceptions;
using StriderWebApi.Services.Interfaces;

namespace StriderWebApi.Services
{
    public class MicrocycleService(
        IMicrocycleRepository microcycleRepository,
        ITrainingSessionsRepository trainingSessionRepository,
        IMesocycleRepository mesocycleRepository,
        IPlanningRepository planningRepository) : IMicrocycleService
    {
        public async Task<MicrocycleResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var microcycle = await microcycleRepository.GetByIdAsync(id, cancellationToken);
            if (microcycle == null)
                throw new NotFoundException("Microciclo no encontrado");

            return MapToMicrocycleResponseDto(microcycle);
        }

        public async Task<IEnumerable<MicrocycleResponseDto>> GetByMesocycleIdAsync(int mesocycleId, CancellationToken cancellationToken = default)
        {
            var microcycles = await microcycleRepository.GetByMesocycleIdAsync(mesocycleId, cancellationToken);
            return microcycles.Select(MapToMicrocycleResponseDto);
        }

        public async Task<IEnumerable<MicrocycleResponseDto>> GetByPeriodIdAsync(int periodId, CancellationToken cancellationToken = default)
        {
            var microcycles = await microcycleRepository.GetByPeriodIdAsync(periodId, cancellationToken);
            return microcycles.Select(MapToMicrocycleResponseDto);
        }

        public async Task<MicrocycleResponseDto> UpdateAsync(int id, UpdateMicrocycleDto dto, int coachId, CancellationToken cancellationToken = default)
        {
            var microcycle = await microcycleRepository.GetByIdAsync(id, cancellationToken);
            if (microcycle == null)
                throw new NotFoundException("Microciclo no encontrado");

            if (!await ValidateMicrocycleAccessAsync(id, coachId, cancellationToken))
                throw new UnauthorizedException("No tienes permisos para actualizar este microciclo");

            microcycle.Sessions = dto.Sessions;
            microcycle.Volume = dto.Volume;
            microcycle.Intensity = dto.Intensity;
            microcycle.Focus = dto.Focus;

            var updatedMicrocycle = await microcycleRepository.UpdateAsync(microcycle, cancellationToken);
            return MapToMicrocycleResponseDto(updatedMicrocycle);
        }

        public async Task<bool> DeleteAsync(int id, int coachId, CancellationToken cancellationToken = default)
        {
            if (!await ValidateMicrocycleAccessAsync(id, coachId, cancellationToken))
                throw new UnauthorizedException("No tienes permisos para eliminar este microciclo");

            return await microcycleRepository.DeleteAsync(id, cancellationToken);
        }

        public async Task<decimal> RecalculateVolumeAsync(int microcycleId, CancellationToken cancellationToken = default)
        {
            var microcycle = await microcycleRepository.GetByIdAsync(microcycleId, cancellationToken);
            if (microcycle == null)
                throw new NotFoundException("Microciclo no encontrado");

            // Usar el método del repository que calcula el volumen sumando las distancias de los intervalos
            var totalVolume = await microcycleRepository.CalculateTotalVolumeAsync(microcycleId, cancellationToken);

            // Actualizar el volumen del microciclo
            microcycle.Volume = totalVolume;
            await microcycleRepository.UpdateAsync(microcycle, cancellationToken);

            return totalVolume;
        }

        public async Task<bool> UpdateVolumeAutomaticallyAsync(int microcycleId, CancellationToken cancellationToken = default)
        {
            await RecalculateVolumeAsync(microcycleId, cancellationToken);
            return true;
        }

        public async Task<bool> ValidateMicrocycleAccessAsync(int microcycleId, int coachId, CancellationToken cancellationToken = default)
        {
            var microcycle = await microcycleRepository.GetByIdAsync(microcycleId, cancellationToken);
            if (microcycle == null) return false;

            var mesocycle = await mesocycleRepository.GetByIdAsync(microcycle.MesocycleId, cancellationToken);
            if (mesocycle == null) return false;

            var planning = await planningRepository.GetByIdAsync(mesocycle.PlanningId, cancellationToken);
            return planning != null && planning.CoachId == coachId;
        }

        private MicrocycleResponseDto MapToMicrocycleResponseDto(Microcycle microcycle)
        {
            return new MicrocycleResponseDto
            {
                Id = microcycle.Id,
                WeekNumber = microcycle.WeekNumber,
                StartDate = microcycle.StartDate,
                EndDate = microcycle.EndDate,
                Sessions = microcycle.Sessions,
                Volume = microcycle.Volume,
                Intensity = microcycle.Intensity,
                Focus = microcycle.Focus,
                MesocycleId = microcycle.MesocycleId,
                PeriodId = microcycle.PeriodId,
                TrainingSessionsCount = microcycle.TrainingSessions?.Count ?? 0,
                CreatedAt = microcycle.CreatedAt,
                UpdatedAt = microcycle.UpdatedAt
            };
        }
    }
}
