using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Domain.Enums;
using StriderWebApi.Dto.Trainings;
using StriderWebApi.Exceptions;
using StriderWebApi.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace StriderWebApi.Services
{
    public class TrainingSessionService(
         ITrainingSessionsRepository trainingSessionRepository,
         IMicrocycleRepository microcycleRepository,
         IPlanningRepository planningRepository,
         IMicrocycleService microcycleService,
         ITrainingSessionAthleteRepository trainingSessionAthleteRepository,
         ITrainingTemplateRepository trainingTemplateRepository) : ITrainingSessionService
    {
        public async Task<TrainingSessionResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var session = await trainingSessionRepository.GetByIdWithAthletesAsync(id, cancellationToken);
            if (session == null)
            {
                throw new NotFoundException("Sesión de entrenamiento no encontrada");
            }

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

        public Task<TrainingSessionResponseDto> CreateAsync(CreateTrainingSessionDto dto, int coachId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Use CreateWithAutoMicrocycleDetectionAsync instead");
        }

        public async Task<TrainingSessionResponseDto> CreateWithAutoMicrocycleDetectionAsync(
            CreateTrainingSessionDto dto,
            int coachId,
            CancellationToken cancellationToken = default)
        {
            ValidateSeriesOrThrow(dto.Series);

            var planning = await planningRepository.GetByIdAsync(dto.PlanningId, cancellationToken);
            if (planning == null)
            {
                throw new NotFoundException("Planificación no encontrada");
            }

            if (planning.CoachId != coachId)
            {
                throw new UnauthorizedException("No tienes permisos para crear sesiones en esta planificación");
            }

            var sessionDate = dto.Date.Kind == DateTimeKind.Utc
                ? dto.Date
                : DateTime.SpecifyKind(dto.Date, DateTimeKind.Utc);

            var microcycle = await microcycleRepository.GetByPlanningIdAndDateAsync(dto.PlanningId, sessionDate, cancellationToken);
            if (microcycle == null)
            {
                throw new ValidationException(
                    $"No se encontró un microciclo para la fecha {dto.Date:yyyy-MM-dd} en la planificación. " +
                    "Asegúrate de que la fecha esté dentro del rango de algún microciclo de la planificación.");
            }

            if (sessionDate < microcycle.StartDate || sessionDate > microcycle.EndDate)
            {
                throw new ValidationException(
                    $"La fecha {dto.Date:yyyy-MM-dd} no está dentro del rango del microciclo " +
                    $"({microcycle.StartDate:yyyy-MM-dd} - {microcycle.EndDate:yyyy-MM-dd})");
            }

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

            var series = MapSeriesFromDtos(dto.Series!, sessionId: createdSession.Id, templateId: null);
            await trainingTemplateRepository.AddSeriesAsync(series, cancellationToken);

            if (dto.AthleteIds != null)
            {
                foreach (var athleteId in dto.AthleteIds)
                {
                    var sessionAthlete = new TrainingSessionAthlete
                    {
                        TrainingSessionId = createdSession.Id,
                        AthleteId = athleteId,
                        Status = SessionStatus.Pending
                    };
                    await trainingSessionAthleteRepository.CreateAsync(sessionAthlete, cancellationToken);
                }
            }

            await RecalculateSessionVolumeAsync(createdSession.Id, cancellationToken);
            await microcycleService.UpdateVolumeAutomaticallyAsync(microcycle.Id, cancellationToken);

            var sessionsCount = await trainingSessionRepository.GetByMicrocycleIdAsync(microcycle.Id, cancellationToken);
            microcycle.Sessions = sessionsCount.Count();
            await microcycleRepository.UpdateAsync(microcycle, cancellationToken);

            var sessionWithSeries = await trainingSessionRepository.GetByIdWithAthletesAsync(createdSession.Id, cancellationToken);
            return MapToTrainingSessionResponseDto(sessionWithSeries ?? createdSession);
        }

        public async Task<TrainingSessionResponseDto> UpdateAsync(int id, UpdateTrainingSessionDto dto, int coachId, CancellationToken cancellationToken = default)
        {
            ValidateSeriesOrThrow(dto.Series);

            var session = await trainingSessionRepository.GetByIdAsync(id, cancellationToken);
            if (session == null)
            {
                throw new NotFoundException("Sesión de entrenamiento no encontrada");
            }

            if (!await ValidateSessionAccessAsync(id, coachId, cancellationToken))
            {
                throw new UnauthorizedException("No tienes permisos para actualizar esta sesión");
            }

            session.Name = dto.Name;
            session.Description = dto.Description;
            session.Date = dto.Date;
            session.Category = dto.Category;
            session.Notes = dto.Notes;

            session = await trainingSessionRepository.UpdateAsync(session, cancellationToken);

            var series = MapSeriesFromDtos(dto.Series!, sessionId: session.Id, templateId: null);
            await trainingTemplateRepository.ReplaceSeriesForSessionAsync(session.Id, series, cancellationToken);

            await RecalculateSessionVolumeAsync(session.Id, cancellationToken);
            await microcycleService.UpdateVolumeAutomaticallyAsync(session.MicrocycleId, cancellationToken);

            var sessionWithSeries = await trainingSessionRepository.GetByIdWithAthletesAsync(session.Id, cancellationToken);
            return MapToTrainingSessionResponseDto(sessionWithSeries ?? session);
        }

        public async Task<bool> DeleteAsync(int id, int coachId, CancellationToken cancellationToken = default)
        {
            var session = await trainingSessionRepository.GetByIdAsync(id, cancellationToken);
            if (session == null)
            {
                return false;
            }

            if (!await ValidateSessionAccessAsync(id, coachId, cancellationToken))
            {
                throw new UnauthorizedException("No tienes permisos para eliminar esta sesión");
            }

            var microcycleId = session.MicrocycleId;
            var result = await trainingSessionRepository.DeleteAsync(id, cancellationToken);

            if (result)
            {
                await microcycleService.UpdateVolumeAutomaticallyAsync(microcycleId, cancellationToken);
            }

            return result;
        }

        public async Task<bool> ValidateSessionAccessAsync(int sessionId, int coachId, CancellationToken cancellationToken = default)
        {
            var session = await trainingSessionRepository.GetByIdAsync(sessionId, cancellationToken);
            if (session == null)
            {
                return false;
            }

            var planning = await planningRepository.GetByIdAsync(session.PlanningId, cancellationToken);
            return planning != null && planning.CoachId == coachId;
        }

        public async Task<bool> ValidateDateInMicrocycleRangeAsync(int microcycleId, DateTime date, CancellationToken cancellationToken = default)
        {
            var microcycle = await microcycleRepository.GetByIdAsync(microcycleId, cancellationToken);
            if (microcycle == null)
            {
                return false;
            }

            return date >= microcycle.StartDate && date <= microcycle.EndDate;
        }

        public async Task<IEnumerable<TrainingSessionResponseDto>> GetByAthleteIdAsync(int athleteId, int? planningId = null, CancellationToken cancellationToken = default)
        {
            var sessions = await trainingSessionRepository.GetByAthleteIdAsync(athleteId, planningId, cancellationToken);
            return sessions.Select(MapToTrainingSessionResponseDto);
        }

        public async Task<decimal> RecalculateSessionVolumeAsync(int sessionId, CancellationToken cancellationToken = default)
        {
            var series = await trainingTemplateRepository.GetSeriesBySessionIdAsync(sessionId, cancellationToken);
            decimal totalDistanceMeters = 0;

            foreach (var set in series)
            {
                if (set.Intervals == null)
                {
                    continue;
                }

                totalDistanceMeters += set.Intervals.Sum(interval => interval.Distance * interval.Repetitions);
            }

            return totalDistanceMeters / 1000m;
        }

        private static void ValidateSeriesOrThrow(List<CreateTrainingSeriesDto>? series)
        {
            if (series == null || series.Count == 0)
            {
                throw new ValidationException("La sesión debe incluir al menos una serie con intervalos.");
            }

            for (int seriesIndex = 0; seriesIndex < series.Count; seriesIndex++)
            {
                if (series[seriesIndex].Intervals == null || series[seriesIndex].Intervals.Count == 0)
                {
                    throw new ValidationException($"La serie {seriesIndex + 1} debe contener al menos un intervalo.");
                }
            }
        }

        private static List<TrainingSeries> MapSeriesFromDtos(List<CreateTrainingSeriesDto> seriesDtos, int? sessionId, int? templateId)
        {
            var list = new List<TrainingSeries>();
            var now = DateTime.UtcNow;

            foreach (var seriesDto in seriesDtos.OrderBy(s => s.OrderIndex))
            {
                var series = new TrainingSeries
                {
                    TrainingSessionId = sessionId,
                    TrainingTemplateId = templateId,
                    Name = seriesDto.Name.Trim(),
                    Repetitions = seriesDto.Repetitions,
                    RecoveryBetweenSets = seriesDto.RecoveryBetweenSets?.Trim() ?? "00:00",
                    OrderIndex = seriesDto.OrderIndex,
                    Notes = seriesDto.Notes?.Trim(),
                    CreatedAt = now,
                    UpdatedAt = now,
                    Intervals = seriesDto.Intervals
                        .OrderBy(i => i.OrderIndex)
                        .Select(intervalDto => new TrainingInterval
                        {
                            Type = intervalDto.Type,
                            Repetitions = intervalDto.Repetitions,
                            Distance = intervalDto.Distance,
                            TargetTime = intervalDto.TargetTime?.Trim(),
                            RecoveryTime = intervalDto.RecoveryTime?.Trim() ?? "00:00",
                            PaceType = intervalDto.PaceType,
                            Pace = intervalDto.Pace,
                            Vo2MaxPercentage = intervalDto.Vo2MaxPercentage,
                            Description = intervalDto.Description?.Trim(),
                            Intensity = intervalDto.Intensity,
                            TrainingMode = intervalDto.TrainingMode,
                            Duration = intervalDto.Duration?.Trim(),
                            TargetSpeed = intervalDto.TargetSpeed?.Trim(),
                            OrderIndex = intervalDto.OrderIndex,
                            CreatedAt = now,
                            UpdatedAt = now
                        })
                        .ToList()
                };

                list.Add(series);
            }

            return list;
        }

        private TrainingSessionResponseDto MapToTrainingSessionResponseDto(TrainingSession session)
        {
            decimal sessionVolume = 0;

            if (session.Series != null)
            {
                foreach (var set in session.Series)
                {
                    if (set.Intervals == null)
                    {
                        continue;
                    }

                    sessionVolume += set.Intervals.Sum(interval => (interval.Distance * interval.Repetitions) / 1000m);
                }
            }

            var orderedSeries = session.Series?.OrderBy(s => s.OrderIndex).ToList() ?? new List<TrainingSeries>();
            var orderedIntervals = orderedSeries
                .SelectMany(s => (s.Intervals ?? new List<TrainingInterval>()).OrderBy(i => i.OrderIndex))
                .ToList();

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
                Series = orderedSeries
                    .Select(series => new TrainingSeriesResponseDto
                    {
                        Id = series.Id,
                        Name = series.Name,
                        Repetitions = series.Repetitions,
                        RecoveryBetweenSets = series.RecoveryBetweenSets,
                        OrderIndex = series.OrderIndex,
                        Notes = series.Notes,
                        Intervals = (series.Intervals ?? new List<TrainingInterval>())
                            .OrderBy(interval => interval.OrderIndex)
                            .Select(MapToTrainingIntervalResponseDto)
                            .ToList()
                    })
                    .ToList(),
                StructureType = DetermineStructureType(session),
                Notes = session.Notes,
                Volume = sessionVolume,
                CreatedAt = session.CreatedAt,
                UpdatedAt = session.UpdatedAt
            };
        }

        private static TrainingIntervalResponseDto MapToTrainingIntervalResponseDto(TrainingInterval interval)
        {
            return new TrainingIntervalResponseDto
            {
                Id = interval.Id,
                Type = interval.Type,
                Repetitions = interval.Repetitions,
                Distance = interval.Distance,
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

        private static string DetermineStructureType(TrainingSession session)
        {
            if (session.Series == null || session.Series.Count == 0)
            {
                return "simple";
            }

            if (session.Series.Count > 1)
            {
                return "advanced";
            }

            var singleSeries = session.Series.First();

            var isDefaultName = string.Equals(singleSeries.Name, "Intervalos Simples", StringComparison.OrdinalIgnoreCase) ||
                                string.Equals(singleSeries.Name, "Serie Principal", StringComparison.OrdinalIgnoreCase);

            var isDefaultRepetitions = singleSeries.Repetitions <= 1;
            var isDefaultRecovery = string.Equals(singleSeries.RecoveryBetweenSets, "00:00", StringComparison.OrdinalIgnoreCase);
            var hasNotes = !string.IsNullOrWhiteSpace(singleSeries.Notes);

            if (!isDefaultName || !isDefaultRepetitions || !isDefaultRecovery || hasNotes)
            {
                return "advanced";
            }

            return "simple";
        }
    }
}
