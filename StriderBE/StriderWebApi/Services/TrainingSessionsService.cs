using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Domain.Enums;
using StriderWebApi.Dto.Trainings;
using StriderWebApi.Exceptions;
using StriderWebApi.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Xml;

namespace StriderWebApi.Services
{
    public class TrainingSessionService(
         ITrainingSessionsRepository trainingSessionRepository,
         IMicrocycleRepository microcycleRepository,
         IPlanningRepository planningRepository,
         IMicrocycleService microcycleService,
         ITrainingSessionAthleteRepository trainingSessionAthleteRepository,
         ITrainingTemplateRepository trainingTemplateRepository,
         IUserRepository userRepository) : ITrainingSessionService
    {
        public async Task<TrainingSessionResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var session = await trainingSessionRepository.GetByIdWithAthletesAsync(id, cancellationToken);
            if (session == null)
            {
                throw new NotFoundException("Sesión de entrenamiento no encontrada");
            }

            return await MapToTrainingSessionResponseDtoAsync(session, null, cancellationToken);
        }

        public async Task<IEnumerable<TrainingSessionResponseDto>> GetByPlanningIdAsync(int planningId, CancellationToken cancellationToken = default)
        {
            var sessions = await trainingSessionRepository.GetByPlanningIdAsync(planningId, cancellationToken);
            var result = new List<TrainingSessionResponseDto>();
            foreach (var session in sessions)
            {
                result.Add(await MapToTrainingSessionResponseDtoAsync(session, null, cancellationToken));
            }
            return result;
        }

        public async Task<IEnumerable<TrainingSessionResponseDto>> GetByMicrocycleIdAsync(int microcycleId, CancellationToken cancellationToken = default)
        {
            var sessions = await trainingSessionRepository.GetByMicrocycleIdAsync(microcycleId, cancellationToken);
            var result = new List<TrainingSessionResponseDto>();
            foreach (var session in sessions)
            {
                result.Add(await MapToTrainingSessionResponseDtoAsync(session, null, cancellationToken));
            }
            return result;
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
            return await MapToTrainingSessionResponseDtoAsync(sessionWithSeries ?? createdSession, null, cancellationToken);
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

            // Actualizar asignaciones de atletas
            if (dto.AthleteIds != null)
            {
                // Obtener atletas actualmente asignados
                var currentAthletes = await trainingSessionAthleteRepository.GetByTrainingSessionIdAsync(session.Id, cancellationToken);
                var currentAthleteIds = currentAthletes.Select(a => a.AthleteId).ToHashSet();
                var newAthleteIds = dto.AthleteIds.ToHashSet();

                // Identificar atletas a agregar (están en nuevo pero no en actual)
                var athletesToAdd = newAthleteIds.Except(currentAthleteIds).ToList();

                // Identificar atletas a eliminar (están en actual pero no en nuevo)
                var athletesToRemove = currentAthletes
                    .Where(a => !newAthleteIds.Contains(a.AthleteId))
                    .ToList();

                // Eliminar asignaciones que ya no están en la lista
                foreach (var athleteToRemove in athletesToRemove)
                {
                    await trainingSessionAthleteRepository.DeleteAsync(athleteToRemove.Id, cancellationToken);
                }

                // Agregar nuevas asignaciones
                foreach (var athleteId in athletesToAdd)
                {
                    var sessionAthlete = new TrainingSessionAthlete
                    {
                        TrainingSessionId = session.Id,
                        AthleteId = athleteId,
                        Status = SessionStatus.Pending
                    };
                    await trainingSessionAthleteRepository.CreateAsync(sessionAthlete, cancellationToken);
                }
            }

            var series = MapSeriesFromDtos(dto.Series!, sessionId: session.Id, templateId: null);
            await trainingTemplateRepository.ReplaceSeriesForSessionAsync(session.Id, series, cancellationToken);

            await RecalculateSessionVolumeAsync(session.Id, cancellationToken);
            await microcycleService.UpdateVolumeAutomaticallyAsync(session.MicrocycleId, cancellationToken);

            var sessionWithSeries = await trainingSessionRepository.GetByIdWithAthletesAsync(session.Id, cancellationToken);
            return await MapToTrainingSessionResponseDtoAsync(sessionWithSeries ?? session, null, cancellationToken);
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
            var result = new List<TrainingSessionResponseDto>();
            foreach (var session in sessions)
            {
                result.Add(await MapToTrainingSessionResponseDtoAsync(session, athleteId, cancellationToken));
            }
            return result;
        }

        public async Task<IEnumerable<TrainingSessionResponseDto>> GetMyTrainingSessionsAsync(int athleteId, DateTime? date = null, CancellationToken cancellationToken = default)
        {
            IEnumerable<TrainingSession> sessions;

            if (date.HasValue)
            {
                // Filtrar por fecha si se proporciona
                var dateUtc = date.Value.Kind == DateTimeKind.Utc
                    ? date.Value
                    : DateTime.SpecifyKind(date.Value, DateTimeKind.Utc);
                sessions = await trainingSessionRepository.GetByAthleteIdAndDateAsync(athleteId, dateUtc, cancellationToken);
            }
            else
            {
                // Obtener todas las sesiones del atleta
                sessions = await trainingSessionRepository.GetByAthleteIdAsync(athleteId, null, cancellationToken);
            }

            var result = new List<TrainingSessionResponseDto>();
            foreach (var session in sessions)
            {
                result.Add(await MapToTrainingSessionResponseDtoAsync(session, athleteId, cancellationToken));
            }
            return result;
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

                // Calcular la distancia base de la serie (suma de intervalos * repeticiones de intervalo)
                var seriesBaseDistanceMeters = set.Intervals.Sum(interval => (interval.Distance == 0? ParseTimeStringToSeconds(interval.Duration) / interval.Pace : interval.Distance) * interval.Repetitions);
                // Multiplicar por las repeticiones de la serie
                var seriesRepetitions = set.Repetitions > 0 ? set.Repetitions : 1;
                totalDistanceMeters += seriesBaseDistanceMeters ?? 0 * seriesRepetitions;
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

                // Validar intervalos
                for (int intervalIndex = 0; intervalIndex < series[seriesIndex].Intervals.Count; intervalIndex++)
                {
                    var interval = series[seriesIndex].Intervals[intervalIndex];
                    ValidateIntervalPaceType(interval, seriesIndex + 1, intervalIndex + 1);
                }
            }
        }

        private static void ValidateIntervalPaceType(CreateTrainingIntervalDto interval, int seriesIndex, int intervalIndex)
        {
            // Si el intervalo tiene distancia (seleccionado por distancia), validar PaceType
            if (interval.Distance > 0)
            {
                if (interval.PaceType == PaceType.Fixed)
                {
                    // Para Fixed, debe tener Pace o TargetSpeed
                    if (!interval.Pace.HasValue && string.IsNullOrWhiteSpace(interval.TargetSpeed))
                    {
                        throw new ValidationException(
                            $"El intervalo {intervalIndex} de la serie {seriesIndex} tiene PaceType 'Fixed' pero no tiene un valor de velocidad fija (Pace o TargetSpeed).");
                    }
                }
                else if (interval.PaceType == PaceType.Vo2MaxPercentage)
                {
                    // Para Vo2MaxPercentage, debe tener Vo2MaxPercentage
                    if (!interval.Vo2MaxPercentage.HasValue)
                    {
                        throw new ValidationException(
                            $"El intervalo {intervalIndex} de la serie {seriesIndex} tiene PaceType 'Vo2MaxPercentage' pero no tiene un porcentaje de VO2Max especificado.");
                    }

                    // Validar que el porcentaje esté en el rango válido
                    if (interval.Vo2MaxPercentage.Value < 0 || interval.Vo2MaxPercentage.Value > 100)
                    {
                        throw new ValidationException(
                            $"El intervalo {intervalIndex} de la serie {seriesIndex} tiene un porcentaje de VO2Max inválido. Debe estar entre 0 y 100.");
                    }
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
                    Repetitions = seriesDto.Repetitions > 0 ? seriesDto.Repetitions : 1,
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
                            Repetitions = intervalDto.Repetitions > 0 ? intervalDto.Repetitions : 1,
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

        private async Task<TrainingSessionResponseDto> MapToTrainingSessionResponseDtoAsync(TrainingSession session, int? athleteId = null, CancellationToken cancellationToken = default)
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

                    // Calcular la distancia base de la serie (suma de intervalos * repeticiones de intervalo)
                    var seriesBaseDistanceMeters = set.Intervals.Sum(interval => interval.Distance * interval.Repetitions);
                    // Multiplicar por las repeticiones de la serie
                    var seriesRepetitions = set.Repetitions > 0 ? set.Repetitions : 1;
                    sessionVolume += (seriesBaseDistanceMeters * seriesRepetitions) / 1000m;
                }
            }

            var orderedSeries = session.Series?.OrderBy(s => s.OrderIndex).ToList() ?? new List<TrainingSeries>();
            var orderedIntervals = orderedSeries
                .SelectMany(s => (s.Intervals ?? new List<TrainingInterval>()).OrderBy(i => i.OrderIndex))
                .ToList();
            
            // Obtener VO2Max del atleta si está disponible
            string? athleteVO2Max = null;
            if (athleteId.HasValue)
            {
                var athlete = await userRepository.GetUserByIdAsync(athleteId.Value);
                if (athlete is Athlete athleteUser)
                {
                    athleteVO2Max = athleteUser.VO2Max;
                }
            }
            
            var (estimatedWorkSeconds, estimatedRecoverySeconds) = CalculateEstimatedTimes(orderedSeries, athleteVO2Max);
            var structureType = DetermineStructureType(session);

            // Obtener el TrainingSessionAthleteId si se proporciona un athleteId específico
            int? trainingSessionAthleteId = null;
            bool hasCompletedWorkout = false;
            if (athleteId.HasValue && session.Athletes != null)
            {
                var trainingSessionAthlete = session.Athletes.FirstOrDefault(a => a.AthleteId == athleteId.Value);
                trainingSessionAthleteId = trainingSessionAthlete?.Id;
                
                // Verificar si hay un completedWorkout para esta sesión y fecha (ya está incluido en el query)
                if (trainingSessionAthlete != null && trainingSessionAthlete.CompletedWorkouts != null)
                {
                    var sessionDate = session.Date.Date;
                    hasCompletedWorkout = trainingSessionAthlete.CompletedWorkouts
                        .Any(cw => cw.Date.Date == sessionDate);
                }
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
                TrainingSessionAthleteId = trainingSessionAthleteId,
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
                Intervals = orderedIntervals
                    .Select(MapToTrainingIntervalResponseDto)
                    .ToList(),
                StructureType = structureType,
                Notes = session.Notes,
                Volume = sessionVolume,
                EstimatedWorkSeconds = estimatedWorkSeconds,
                EstimatedRecoverySeconds = estimatedRecoverySeconds,
                CreatedAt = session.CreatedAt,
                UpdatedAt = session.UpdatedAt,
                HasCompletedWorkout = hasCompletedWorkout
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

        private static (int WorkSeconds, int RecoverySeconds) CalculateEstimatedTimes(IEnumerable<TrainingSeries> seriesCollection, string? athleteVO2Max = null)
        {
            var totalWorkSeconds = 0;
            var totalRecoverySeconds = 0;

            foreach (var series in seriesCollection)
            {
                if (series == null)
                {
                    continue;
                }

                var seriesRepetitions = Math.Max(series.Repetitions, 1);
                var betweenSetsRecoverySeconds = ParseTimeStringToSeconds(series.RecoveryBetweenSets) ?? 0;

                if (betweenSetsRecoverySeconds > 0 && seriesRepetitions > 1)
                {
                    totalRecoverySeconds += betweenSetsRecoverySeconds * (seriesRepetitions - 1);
                }

                if (series.Intervals == null)
                {
                    continue;
                }

                var intervalsList = series.Intervals.Where(i => i != null).ToList();
                var intervalIndex = 0;
                
                foreach (var interval in intervalsList)
                {
                    var intervalRepetitions = Math.Max(interval.Repetitions, 1);
                    var paceSeconds = ParsePaceSeconds(interval, athleteVO2Max);
                    var distanceKm = interval.Distance > 0 ? (double)interval.Distance / 1000d : 0d;

                    if (paceSeconds > 0 && distanceKm > 0)
                    {
                        var perRepetitionSeconds = (int)Math.Round(paceSeconds * distanceKm);
                        totalWorkSeconds += perRepetitionSeconds * intervalRepetitions * seriesRepetitions;
                    }
                    else
                    {
                        var durationSeconds = ParseTimeStringToSeconds(interval.Duration)
                            ?? ParseTimeStringToSeconds(interval.TargetTime);

                        if (durationSeconds.HasValue && durationSeconds.Value > 0)
                        {
                            totalWorkSeconds += durationSeconds.Value * intervalRepetitions * seriesRepetitions;
                        }
                    }

                    var intervalRecoverySeconds = ParseTimeStringToSeconds(interval.RecoveryTime) ?? 0;
                    if (intervalRecoverySeconds > 0)
                    {
                        // Sumar recuperación entre repeticiones del mismo intervalo: (repeticiones - 1) veces
                        // Si hay 2 repeticiones, hay 1 recuperación entre ellas
                        // Si hay 3 repeticiones, hay 2 recuperaciones entre ellas
                        var recoveryBetweenRepetitions = intervalRepetitions > 1 ? (intervalRepetitions - 1) : 0;
                        totalRecoverySeconds += intervalRecoverySeconds * recoveryBetweenRepetitions * seriesRepetitions;
                        
                        // Sumar recuperación después de cada intervalo (excepto el último intervalo de la serie)
                        // La recuperación después del intervalo se aplica una vez por cada repetición de la serie
                        var isLastInterval = intervalIndex == intervalsList.Count - 1;
                        if (!isLastInterval)
                        {
                            // Si hay múltiples intervalos, cada intervalo (excepto el último) tiene recuperación después
                            totalRecoverySeconds += intervalRecoverySeconds * seriesRepetitions;
                        }
                        else if (intervalsList.Count == 1)
                        {
                            // Si hay solo un intervalo en la serie, sumar la recuperación una vez
                            // (representa la recuperación final después del único intervalo)
                            totalRecoverySeconds += intervalRecoverySeconds * seriesRepetitions;
                        }
                    }
                    
                    intervalIndex++;
                }
            }

            return (totalWorkSeconds, totalRecoverySeconds);
        }

        private static int ParsePaceSeconds(TrainingInterval interval, string? athleteVO2Max = null)
        {
            if (!string.IsNullOrWhiteSpace(interval.TargetSpeed))
            {
                var sanitized = interval.TargetSpeed
                    .Replace("min/km", string.Empty, StringComparison.OrdinalIgnoreCase)
                    .Trim();

                var candidate = sanitized.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .FirstOrDefault();

                var seconds = ParseTimeStringToSeconds(candidate);
                if (seconds.HasValue && seconds.Value > 0)
                {
                    return seconds.Value;
                }
            }

            // Si el paceType es Vo2MaxPercentage y tenemos el VO2Max del atleta, calcular el ritmo
            if (interval.PaceType == PaceType.Vo2MaxPercentage && 
                interval.Vo2MaxPercentage.HasValue && 
                !string.IsNullOrWhiteSpace(athleteVO2Max))
            {
                var vo2MaxSeconds = ParseTimeStringToSeconds(athleteVO2Max);
                if (vo2MaxSeconds.HasValue && vo2MaxSeconds.Value > 0)
                {
                    // Calcular ritmo objetivo: ritmo_vo2max / (porcentaje / 100)
                    // Ejemplo: VO2Max = 3:30 (210 seg), porcentaje = 100% -> ritmo = 210 / 1.0 = 210 seg
                    // Ejemplo: VO2Max = 3:30 (210 seg), porcentaje = 80% -> ritmo = 210 / 0.8 = 262.5 seg
                    var percentage = (double)interval.Vo2MaxPercentage.Value / 100.0;
                    if (percentage > 0)
                    {
                        var targetPaceSeconds = (int)Math.Round(vo2MaxSeconds.Value / percentage);
                        return targetPaceSeconds;
                    }
                }
            }

            var fallbackTargetTime = ParseTimeStringToSeconds(interval.TargetTime);
            if (fallbackTargetTime.HasValue && fallbackTargetTime.Value > 0)
            {
                return fallbackTargetTime.Value;
            }

            var fallbackDuration = ParseTimeStringToSeconds(interval.Duration);
            if (fallbackDuration.HasValue && fallbackDuration.Value > 0)
            {
                return fallbackDuration.Value;
            }

            return 0;
        }

        private static int? ParseTimeStringToSeconds(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var trimmed = value.Trim();

            if (trimmed.StartsWith("PT", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var timeSpan = XmlConvert.ToTimeSpan(trimmed);
                    return (int)Math.Round(timeSpan.TotalSeconds);
                }
                catch
                {
                    // Ignorar errores de formato ISO y continuar con otras estrategias de parseo
                }
            }

            if (trimmed.Contains(':'))
            {
                var parts = trimmed.Split(':');

                if (parts.Length == 2 && int.TryParse(parts[0], out var minutes) && int.TryParse(parts[1], out var seconds))
                {
                    return minutes * 60 + seconds;
                }

                if (parts.Length == 3
                    && int.TryParse(parts[0], out var hours)
                    && int.TryParse(parts[1], out var partMinutes)
                    && int.TryParse(parts[2], out var partSeconds))
                {
                    return hours * 3600 + partMinutes * 60 + partSeconds;
                }
            }

            if (double.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, out var numericMinutes))
            {
                return (int)Math.Round(numericMinutes * 60);
            }

            return null;
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
