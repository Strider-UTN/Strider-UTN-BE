using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Domain.Enums;
using StriderWebApi.Dto.Trainings;
using StriderWebApi.Exceptions;
using StriderWebApi.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using static StriderWebApi.Dto.Trainings.TrainingTemplateDto;

namespace StriderWebApi.Services
{
    public class TrainingSessionService(
        ITrainingSessionsRepository trainingSessionRepository,
        IMicrocycleRepository microcycleRepository,
        IPlanningRepository planningRepository,
        IMicrocycleService microcycleService,
        ITrainingSessionAthleteRepository trainingSessionAthleteRepository,
        ITrainingIntervalRepository trainingIntervalRepository) : ITrainingSessionService
    {
        public async Task<TrainingSessionResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var session = await trainingSessionRepository.GetByIdWithAthletesAsync(id, cancellationToken);
            if (session == null)
                throw new NotFoundException("Sesión de entrenamiento no encontrada");

            return MapToTrainingSessionResponseDto(session);
        }

        public async Task<IEnumerable<TrainingSessionResponseDto>> GetByPlanningIdAsync(int planningId, CancellationToken cancellationToken = default)
        {
            var sessions = await trainingSessionRepository.GetByPlanningIdAsync(planningId, cancellationToken);
            return sessions.Select(MapToTrainingSessionResponseDto);
        }

        public async Task<IEnumerable<TrainingSessionResponseDto>> GetByMicrocycleIdAsync(int microcycleId, CancellationToken cancellationToken = default)
        {
            var sessions = await trainingSessionRepository.GetByMicrocycleIdAsync(microcycleId, cancellationToken);
            return sessions.Select(MapToTrainingSessionResponseDto);
        }

        public async Task<TrainingSessionResponseDto> CreateAsync(CreateTrainingSessionDto dto, int coachId, CancellationToken cancellationToken = default)
        {
            // Este método requiere que se pase el MicrocycleId explícitamente
            // Para la creación automática, usar CreateWithAutoMicrocycleDetectionAsync
            throw new NotImplementedException("Use CreateWithAutoMicrocycleDetectionAsync instead");
        }

        public async Task<TrainingSessionResponseDto> CreateWithAutoMicrocycleDetectionAsync(
            CreateTrainingSessionDto dto,
            int coachId,
            CancellationToken cancellationToken = default)
        {
            var planning = await planningRepository.GetByIdAsync(dto.PlanningId, cancellationToken);
            if (planning == null)
                throw new NotFoundException("Planificación no encontrada");

            if (planning.CoachId != coachId)
                throw new UnauthorizedException("No tienes permisos para crear sesiones en esta planificación");

            var sessionDate = dto.Date.Kind == DateTimeKind.Utc
                ? dto.Date
                : DateTime.SpecifyKind(dto.Date, DateTimeKind.Utc);

            var microcycle = await microcycleRepository.GetByPlanningIdAndDateAsync(
                dto.PlanningId, sessionDate, cancellationToken);

            if (microcycle == null)
                throw new ValidationException(
                    $"No se encontró un microciclo para la fecha {dto.Date:yyyy-MM-dd} en la planificación. " +
                    $"Asegúrate de que la fecha esté dentro del rango de algún microciclo de la planificación.");

            if (sessionDate < microcycle.StartDate || sessionDate > microcycle.EndDate)
                throw new ValidationException(
                    $"La fecha {dto.Date:yyyy-MM-dd} no está dentro del rango del microciclo " +
                    $"({microcycle.StartDate:yyyy-MM-dd} - {microcycle.EndDate:yyyy-MM-dd})");

            var trainingSession = new TrainingSession
            {
                PlanningId = dto.PlanningId,
                MicrocycleId = microcycle.Id,
                Date = sessionDate,
                Name = dto.Name,
                Description = dto.Description,
                Category = dto.Category,
                Notes = dto.Notes,
                CreatedByUserId = coachId
            };

            var createdSession = await trainingSessionRepository.CreateAsync(trainingSession, cancellationToken);

            // Crear intervalos si existen
            if (dto.Intervals != null && dto.Intervals.Any())
            {
                var intervals = new List<TrainingInterval>();
                foreach (var intervalDto in dto.Intervals)
                {
                    var interval = new TrainingInterval
                    {
                        TrainingSessionId = createdSession.Id,
                        Type = intervalDto.Type,
                        Repetitions = intervalDto.Repetitions,
                        Distance = intervalDto.Distance,
                        TargetTime = intervalDto.TargetTime,
                        RecoveryTime = intervalDto.RecoveryTime,
                        PaceType = intervalDto.PaceType,
                        Pace = intervalDto.Pace,
                        Vo2MaxPercentage = intervalDto.Vo2MaxPercentage,
                        Description = intervalDto.Description,
                        Intensity = intervalDto.Intensity,
                        TrainingMode = intervalDto.TrainingMode,
                        Duration = intervalDto.Duration,
                        TargetSpeed = intervalDto.TargetSpeed,
                        OrderIndex = intervalDto.OrderIndex
                    };
                    intervals.Add(interval);
                }
                await trainingIntervalRepository.CreateMultipleAsync(intervals, cancellationToken);
            }

            // Calcular y actualizar volumen de la sesión basado en los intervalos
            await RecalculateSessionVolumeAsync(createdSession.Id, cancellationToken);

            // Asignar atletas a la sesión
            if (dto.AthleteIds != null && dto.AthleteIds.Any())
            {
                foreach (var athleteId in dto.AthleteIds)
                {
                    var sessionAthlete = new TrainingSessionAthlete
                    {
                        TrainingSessionId = createdSession.Id,
                        AthleteId = athleteId,
                        Status = SessionStatus.Pending // Estado inicial
                    };
                    await trainingSessionAthleteRepository.CreateAsync(sessionAthlete, cancellationToken);
                }
            }

            // Recalcular volumen del microciclo automáticamente
            await microcycleService.UpdateVolumeAutomaticallyAsync(microcycle.Id, cancellationToken);

            // Actualizar contador de sesiones del microciclo
            var sessionsCount = await trainingSessionRepository.GetByMicrocycleIdAsync(microcycle.Id, cancellationToken);
            microcycle.Sessions = sessionsCount.Count();
            await microcycleRepository.UpdateAsync(microcycle, cancellationToken);

            return MapToTrainingSessionResponseDto(createdSession);
        }

        public async Task<TrainingSessionResponseDto> UpdateAsync(int id, UpdateTrainingSessionDto dto, int coachId, CancellationToken cancellationToken = default)
        {
            var session = await trainingSessionRepository.GetByIdAsync(id, cancellationToken);
            if (session == null)
                throw new NotFoundException("Sesión de entrenamiento no encontrada");

            if (!await ValidateSessionAccessAsync(id, coachId, cancellationToken))
                throw new UnauthorizedException("No tienes permisos para actualizar esta sesión");

            session.Name = dto.Name;
            session.Description = dto.Description;
            session.Date = dto.Date;
            session.Category = dto.Category;
            session.Notes = dto.Notes;

            var updatedSession = await trainingSessionRepository.UpdateAsync(session, cancellationToken);

            // Recalcular volumen de la sesión basado en los intervalos
            await RecalculateSessionVolumeAsync(session.Id, cancellationToken);

            // Recalcular volumen del microciclo automáticamente
            await microcycleService.UpdateVolumeAutomaticallyAsync(session.MicrocycleId, cancellationToken);

            return MapToTrainingSessionResponseDto(updatedSession);
        }

        public async Task<bool> DeleteAsync(int id, int coachId, CancellationToken cancellationToken = default)
        {
            var session = await trainingSessionRepository.GetByIdAsync(id, cancellationToken);
            if (session == null) return false;

            if (!await ValidateSessionAccessAsync(id, coachId, cancellationToken))
                throw new UnauthorizedException("No tienes permisos para eliminar esta sesión");

            var microcycleId = session.MicrocycleId;
            var result = await trainingSessionRepository.DeleteAsync(id, cancellationToken);

            // Recalcular volumen del microciclo después de eliminar
            if (result)
            {
                await microcycleService.UpdateVolumeAutomaticallyAsync(microcycleId, cancellationToken);
            }

            return result;
        }

        public async Task<bool> ValidateSessionAccessAsync(int sessionId, int coachId, CancellationToken cancellationToken = default)
        {
            var session = await trainingSessionRepository.GetByIdAsync(sessionId, cancellationToken);
            if (session == null) return false;

            var planning = await planningRepository.GetByIdAsync(session.PlanningId, cancellationToken);
            return planning != null && planning.CoachId == coachId;
        }

        public async Task<bool> ValidateDateInMicrocycleRangeAsync(int microcycleId, DateTime date, CancellationToken cancellationToken = default)
        {
            var microcycle = await microcycleRepository.GetByIdAsync(microcycleId, cancellationToken);
            if (microcycle == null) return false;

            return date >= microcycle.StartDate && date <= microcycle.EndDate;
        }

        public async Task<IEnumerable<TrainingSessionResponseDto>> GetByAthleteIdAsync(int athleteId, int? planningId = null, CancellationToken cancellationToken = default)
        {
            var sessions = await trainingSessionRepository.GetByAthleteIdAsync(athleteId, planningId, cancellationToken);
            return sessions.Select(MapToTrainingSessionResponseDto);
        }

        /// <summary>
        /// Recalcula el volumen de una sesión sumando las distancias de todos sus intervalos
        /// </summary>
        public async Task<decimal> RecalculateSessionVolumeAsync(int sessionId, CancellationToken cancellationToken = default)
        {
            var session = await trainingSessionRepository.GetByIdAsync(sessionId, cancellationToken);
            if (session == null)
                throw new NotFoundException("Sesión de entrenamiento no encontrada");

            // Obtener todos los intervalos de la sesión
            var intervals = await trainingIntervalRepository.GetByTrainingSessionIdAsync(sessionId, cancellationToken);

            // Calcular volumen total: sumar todas las distancias (en metros) y convertir a kilómetros
            decimal totalDistanceMeters = 0;
            foreach (var interval in intervals)
            {
                // Distancia total = distancia del intervalo * repeticiones
                totalDistanceMeters += interval.Distance * interval.Repetitions;
            }

            // Convertir de metros a kilómetros
            decimal totalVolumeKm = totalDistanceMeters / 1000m;

            // Actualizar el volumen de la sesión (si tiene el campo)
            // Si TrainingSession no tiene Volume, podemos almacenarlo en un campo calculado o agregarlo
            // Por ahora, lo devolvemos para que se use en el cálculo del microciclo
            return totalVolumeKm;
        }

        private TrainingSessionResponseDto MapToTrainingSessionResponseDto(TrainingSession session)
        {
            // Calcular volumen de la sesión sumando las distancias de los intervalos
            decimal sessionVolume = 0;
            if (session.Intervals != null && session.Intervals.Any())
            {
                decimal totalDistanceMeters = session.Intervals
                    .Sum(i => i.Distance * i.Repetitions);
                sessionVolume = totalDistanceMeters / 1000m; // Convertir de metros a kilómetros
            }

            return new TrainingSessionResponseDto
            {
                Id = session.Id,
                Name = session.Name,
                Description = session.Description,
                Date = session.Date,
                Category = session.Category,
                PlanningId = session.PlanningId,
                MicrocycleId = session.MicrocycleId,
                AthleteIds = session.Athletes?.Select(a => a.AthleteId).ToList() ?? new List<int>(),
                Notes = session.Notes,
                Volume = sessionVolume, // Volumen calculado dinámicamente desde intervalos (km)
                Intervals = session.Intervals?.Select(i => MapToTrainingIntervalResponseDto(i)).OrderBy(i => i.OrderIndex).ToList(),
                CreatedAt = session.CreatedAt,
                UpdatedAt = session.UpdatedAt
            };
        }

        private TrainingIntervalResponseDto MapToTrainingIntervalResponseDto(TrainingInterval interval)
        {
            return new TrainingIntervalResponseDto
            {
                Id = interval.Id,
                Type = interval.Type,
                Repetitions = interval.Repetitions,
                Distance = interval.Distance, // en metros
                TargetTime = interval.TargetTime,
                RecoveryTime = interval.RecoveryTime,
                PaceType = interval.PaceType,
                Pace = interval.Pace,
                Vo2MaxPercentage = interval.Vo2MaxPercentage,
                Description = interval.Description,
                Intensity = interval.Intensity,
                TrainingMode = interval.TrainingMode,
                Duration = interval.Duration,
                TargetSpeed = interval.TargetSpeed,
                OrderIndex = interval.OrderIndex
            };
        }
    }
}
