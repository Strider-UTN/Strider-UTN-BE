using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Dto.Trainings;
using StriderWebApi.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace StriderWebApi.Services
{
    public class TrainingTemplateService(ITrainingTemplateRepository trainingTemplateRepository, IJwtService jwtService) : ITrainingTemplateService
    {
        public async Task<TrainingTemplateResponseDto?> CreateTrainingTemplateAsync(CreateTrainingTemplateDto dto, CancellationToken cancellationToken)
        {
            ValidateSeriesOrThrow(dto.Series);

            var userId = jwtService.GetCurrentUserId();

            var template = new TrainingTemplate
            {
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim() ?? string.Empty,
                Type = dto.Type,
                Category = dto.Category,
                Duration = dto.Duration,
                Distance = dto.Distance,
                TargetPace = dto.TargetPace?.Trim(),
                TargetHR = dto.TargetHR?.Trim(),
                Notes = dto.Notes?.Trim() ?? string.Empty,
                Difficulty = dto.Difficulty,
                IsFavorite = false,
                UseCount = 0,
                Tags = dto.Tags ?? Array.Empty<string>(),
                WarmUpDuration = dto.WarmUpDuration,
                WarmUpPace = dto.WarmUpPace?.Trim(),
                WarmUpDescription = dto.WarmUpDescription?.Trim(),
                CoolDownDuration = dto.CoolDownDuration,
                CoolDownPace = dto.CoolDownPace?.Trim(),
                CoolDownDescription = dto.CoolDownDescription?.Trim(),
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = userId,
                Series = MapSeriesFromDtos(dto.Series!, templateId: null)
            };

            await trainingTemplateRepository.AddTrainingTemplateAsync(template, cancellationToken);

            var createdTemplate = await trainingTemplateRepository.GetTrainingTemplateByIdAsync(template.Id, cancellationToken);
            return createdTemplate == null ? null : MapToResponseDto(createdTemplate);
        }

        public async Task<TrainingTemplateResponseDto?> GetTrainingTemplateByIdAsync(int templateId, CancellationToken cancellationToken)
        {
            var dbTemplate = await trainingTemplateRepository.GetTrainingTemplateByIdAsync(templateId, cancellationToken);
            return dbTemplate == null ? null : MapToResponseDto(dbTemplate);
        }

        public async Task<List<TrainingTemplateResponseDto>> GetAllTrainingTemplatesAsync(CancellationToken cancellationToken)
        {
            var userId = jwtService.GetCurrentUserId() ?? throw new UnauthorizedAccessException("User ID not found in token.");
            var dbTemplates = await trainingTemplateRepository.GetAllTrainingTemplatesAsync(userId, cancellationToken);
            return dbTemplates.Select(MapToResponseDto).ToList();
        }

        public Task<bool> DeleteTrainingTemplateAsync(int templateId, CancellationToken cancellationToken)
        {
            return trainingTemplateRepository.DeleteTrainingTemplateAsync(templateId, cancellationToken);
        }

        public async Task<TrainingTemplateResponseDto?> UpdateTrainingTemplateAsync(int id, CreateTrainingTemplateDto dto, CancellationToken cancellationToken)
        {
            ValidateSeriesOrThrow(dto.Series);

            var dbTemplate = await trainingTemplateRepository.GetTrainingTemplateByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException($"Training template with ID {id} not found.");

            var newSeries = MapSeriesFromDtos(dto.Series!, templateId: id);
            await trainingTemplateRepository.ReplaceSeriesForTemplateAsync(id, newSeries, cancellationToken);

            dbTemplate = await trainingTemplateRepository.GetTrainingTemplateByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException($"Training template with ID {id} not found after updating series.");

            dbTemplate.Name = dto.Name;
            dbTemplate.Description = dto.Description;
            dbTemplate.Type = dto.Type;
            dbTemplate.Category = dto.Category;
            dbTemplate.Duration = dto.Duration;
            dbTemplate.Difficulty = dto.Difficulty;
            dbTemplate.Distance = dto.Distance;
            dbTemplate.TargetPace = dto.TargetPace;
            dbTemplate.TargetHR = dto.TargetHR;
            dbTemplate.Notes = dto.Notes;
            dbTemplate.Tags = dto.Tags.ToArray() ?? [];
            dbTemplate.UpdatedAt = DateTime.UtcNow;

            await trainingTemplateRepository.UpdateTrainingTemplateAsync(dbTemplate, cancellationToken);

            var updatedTemplate = await trainingTemplateRepository.GetTrainingTemplateByIdAsync(dbTemplate.Id, cancellationToken);
            return updatedTemplate == null ? null : MapToResponseDto(updatedTemplate);
        }

        public async Task<TrainingTemplateResponseDto?> ToggleFavoriteTemplateAsync(int id, CancellationToken cancellationToken)
        {
            var dbTemplate = await trainingTemplateRepository.GetTrainingTemplateByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException($"Training template with ID {id} not found.");

            dbTemplate.IsFavorite = !dbTemplate.IsFavorite;
            dbTemplate.UpdatedAt = DateTime.UtcNow;

            await trainingTemplateRepository.UpdateTrainingTemplateAsync(dbTemplate, cancellationToken);

            return MapToResponseDto(dbTemplate);
        }

        private static void ValidateSeriesOrThrow(List<CreateTrainingSeriesDto>? series)
        {
            if (series == null || series.Count == 0)
            {
                throw new ValidationException("La plantilla debe incluir al menos una serie con intervalos.");
            }

            for (int i = 0; i < series.Count; i++)
            {
                if (series[i].Intervals == null || series[i].Intervals.Count == 0)
                {
                    throw new ValidationException($"La serie {i + 1} debe contener al menos un intervalo.");
                }

                // Validar intervalos
                for (int j = 0; j < series[i].Intervals.Count; j++)
                {
                    var interval = series[i].Intervals[j];
                    ValidateIntervalPaceType(interval, i + 1, j + 1);
                }
            }
        }

        private static void ValidateIntervalPaceType(CreateTrainingIntervalDto interval, int seriesIndex, int intervalIndex)
        {
            // Si el intervalo tiene distancia (seleccionado por distancia), validar PaceType
            if (interval.Distance > 0)
            {
                if (interval.PaceType == Domain.Enums.PaceType.Fixed)
                {
                    // Para Fixed, debe tener Pace o TargetSpeed
                    if (!interval.Pace.HasValue && string.IsNullOrWhiteSpace(interval.TargetSpeed))
                    {
                        throw new ValidationException(
                            $"El intervalo {intervalIndex} de la serie {seriesIndex} tiene PaceType 'Fixed' pero no tiene un valor de velocidad fija (Pace o TargetSpeed).");
                    }
                }
                else if (interval.PaceType == Domain.Enums.PaceType.Vo2MaxPercentage)
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

        private static List<TrainingSeries> MapSeriesFromDtos(List<CreateTrainingSeriesDto> seriesDtos, int? templateId)
        {
            var list = new List<TrainingSeries>();
            var now = DateTime.UtcNow;

            foreach (var seriesDto in seriesDtos.OrderBy(s => s.OrderIndex))
            {
                var series = new TrainingSeries
                {
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

        private TrainingTemplateResponseDto MapToResponseDto(TrainingTemplate template)
        {
            var orderedSeries = template.Series
                .OrderBy(s => s.OrderIndex)
                .ToList();

            var orderedIntervals = orderedSeries
                .SelectMany(s => (s.Intervals ?? new List<TrainingInterval>()).OrderBy(i => i.OrderIndex))
                .ToList();

            return new TrainingTemplateResponseDto
            {
                Id = template.Id,
                Name = template.Name,
                Description = template.Description,
                Type = template.Type,
                Category = template.Category,
                Duration = template.Duration,
                Distance = template.Distance,
                TargetPace = template.TargetPace,
                TargetHR = template.TargetHR,
                Notes = template.Notes,
                Difficulty = template.Difficulty,
                IsFavorite = template.IsFavorite,
                UseCount = template.UseCount,
                CreatedAt = template.CreatedAt,
                LastUsed = template.LastUsed,
                StructureType = DetermineStructureType(template),
                Tags = template.Tags ?? Array.Empty<string>(),
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

        private static string DetermineStructureType(TrainingTemplate template)
        {
            if (template.Series == null || template.Series.Count == 0)
            {
                return "simple";
            }

            if (template.Series.Count > 1)
            {
                return "advanced";
            }

            var singleSeries = template.Series.First();

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
