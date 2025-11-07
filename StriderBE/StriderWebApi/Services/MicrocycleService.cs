using StriderWebApi.Data.Repositories;
using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Dto.Microcycle;
using StriderWebApi.Exceptions;
using StriderWebApi.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

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
            var microcycle = await microcycleRepository.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException("Microciclo no encontrado");

            // Recalcular volumen y sesiones antes de devolver
            await RecalculateVolumeAndSessionsAsync(id, cancellationToken);

            // Obtener el microciclo actualizado
            microcycle = await microcycleRepository.GetByIdAsync(id, cancellationToken);

            return MapToMicrocycleResponseDto(microcycle);
        }

        public async Task<IEnumerable<MicrocycleResponseDto>> GetByMesocycleIdAsync(int mesocycleId, CancellationToken cancellationToken = default)
        {
            var microcycles = await microcycleRepository.GetByMesocycleIdAsync(mesocycleId, cancellationToken);

            // Recalcular volumen y sesiones para cada microciclo
            foreach (var microcycle in microcycles)
            {
                await RecalculateVolumeAndSessionsAsync(microcycle.Id, cancellationToken);
            }

            // Obtener los microciclos actualizados
            microcycles = await microcycleRepository.GetByMesocycleIdAsync(mesocycleId, cancellationToken);

            return microcycles.Select(MapToMicrocycleResponseDto);
        }

        public async Task<IEnumerable<MicrocycleResponseDto>> GetByPeriodIdAsync(int periodId, CancellationToken cancellationToken = default)
        {
            var microcycles = await microcycleRepository.GetByPeriodIdAsync(periodId, cancellationToken);

            // Recalcular volumen y sesiones para cada microciclo
            foreach (var microcycle in microcycles)
            {
                await RecalculateVolumeAndSessionsAsync(microcycle.Id, cancellationToken);
            }

            // Obtener los microciclos actualizados
            microcycles = await microcycleRepository.GetByPeriodIdAsync(periodId, cancellationToken);

            return microcycles.Select(MapToMicrocycleResponseDto);
        }

        public async Task<MicrocycleResponseDto> UpdateAsync(int id, UpdateMicrocycleDto dto, int coachId, CancellationToken cancellationToken = default)
        {
            var microcycle = await microcycleRepository.GetByIdAsync(id, cancellationToken);
            if (microcycle == null)
                throw new NotFoundException("Microciclo no encontrado");

            if (!await ValidateMicrocycleAccessAsync(id, coachId, cancellationToken))
                throw new UnauthorizedException("No tienes permisos para actualizar este microciclo");

            // Validar que el nombre no esté vacío
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ValidationException("El nombre del microciclo es requerido");

            // Actualizar campos editables
            microcycle.Name = dto.Name.Trim();
            microcycle.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
            microcycle.Intensity = dto.Intensity;
            microcycle.Focus = dto.Focus;

            // NO actualizar Sessions y Volume - se calculan automáticamente
            // Recalcular volumen y sesiones antes de guardar
            await RecalculateVolumeAndSessionsAsync(microcycle.Id, cancellationToken);

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

            // También recalcular sesiones
            var sessions = await trainingSessionRepository.GetByMicrocycleIdAsync(microcycleId, cancellationToken);
            microcycle.Sessions = sessions.Count();

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

        /// <summary>
        /// Recalcula el volumen y la cantidad de sesiones de un microciclo basándose en las sesiones asignadas
        /// </summary>
        private async Task RecalculateVolumeAndSessionsAsync(int microcycleId, CancellationToken cancellationToken = default)
        {
            var microcycle = await microcycleRepository.GetByIdAsync(microcycleId, cancellationToken);
            if (microcycle == null)
                throw new NotFoundException("Microciclo no encontrado");

            // Obtener todas las sesiones del microciclo para contar
            var sessions = await trainingSessionRepository.GetByMicrocycleIdAsync(microcycleId, cancellationToken);

            // Calcular cantidad de sesiones
            microcycle.Sessions = sessions.Count();

            // Calcular volumen total usando el método del repository
            var totalVolumeKm = await microcycleRepository.CalculateTotalVolumeAsync(microcycleId, cancellationToken);

            // Actualizar el volumen del microciclo
            microcycle.Volume = totalVolumeKm;

            // Guardar los cambios
            await microcycleRepository.UpdateAsync(microcycle, cancellationToken);
        }

        private MicrocycleResponseDto MapToMicrocycleResponseDto(Microcycle microcycle)
        {
            return new MicrocycleResponseDto
            {
                Id = microcycle.Id,
                Name = microcycle.Name,
                Description = microcycle.Description,
                WeekNumber = microcycle.WeekNumber,
                StartDate = microcycle.StartDate,
                EndDate = microcycle.EndDate,
                Sessions = microcycle.Sessions, // Calculado automáticamente
                Volume = microcycle.Volume, // Calculado automáticamente
                Intensity = microcycle.Intensity,
                Focus = microcycle.Focus,
                MesocycleId = microcycle.MesocycleId,
                TrainingSessionsCount = microcycle.TrainingSessions?.Count ?? 0,
                CreatedAt = microcycle.CreatedAt,
                UpdatedAt = microcycle.UpdatedAt
            };
        }
    }
}
