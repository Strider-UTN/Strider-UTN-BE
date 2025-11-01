using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Domain.Enums;
using StriderWebApi.Services.Interfaces;
using static StriderWebApi.Dto.Trainings.TrainingTemplateDto;

namespace StriderWebApi.Services
{
    public class TrainingTemplateService(ITrainingTemplateRepository trainingTemplateRepository, IJwtService jwtService) : ITrainingTemplateService
    {
        public async Task<TrainingTemplateResponseDto?> CreateTrainingTemplateAsync(CreateTrainingTemplateDto dto, CancellationToken cancellationToken)
        {
            var userId = jwtService.GetCurrentUserId();

            // Crear la entidad de plantilla
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
                CreatedByUserId = userId
            };

            // Agregar intervalos si existen
            if (dto.Intervals != null && dto.Intervals.Any())
            {
                template.Intervals = dto.Intervals
                    .Select((intervalDto, index) => new TrainingInterval
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
                        OrderIndex = intervalDto.OrderIndex >= 0
                            ? intervalDto.OrderIndex
                            : index, // Usar índice si no se proporciona OrderIndex
                        CreatedAt = DateTime.UtcNow
                    })
                    .ToList();
            }

            // Guardar la plantilla en el repositorio
            await trainingTemplateRepository.AddTrainingTemplateAsync(template, cancellationToken);


            // Recuperar la plantilla creada con sus intervalos
            var createdTemplate = await trainingTemplateRepository.GetTrainingTemplateByIdAsync(template.Id, cancellationToken);

            if (createdTemplate == null)
                return null;

            // Mapear a DTO de respuesta
            var responseDto = MapToResponseDto(createdTemplate);

            return responseDto;
        }

        public async Task<TrainingTemplateResponseDto?> GetTrainingTemplateByIdAsync(int templateId, CancellationToken cancellationToken)
        {
            var dbTemplate = await trainingTemplateRepository.GetTrainingTemplateByIdAsync(templateId, cancellationToken);
            if (dbTemplate == null)
                return null;
            return MapToResponseDto(dbTemplate);
        }

        public async Task<List<TrainingTemplateResponseDto>> GetAllTrainingTemplatesAsync(CancellationToken cancellationToken)
        {
            var userId = jwtService.GetCurrentUserId();

            if (userId == null) 
                throw new UnauthorizedAccessException("User ID not found in token.");

            var dbTemplates = await trainingTemplateRepository.GetAllTrainingTemplatesAsync(userId.Value, cancellationToken);

            return dbTemplates.Select(MapToResponseDto).ToList();
        }

        public async Task<bool> DeleteTrainingTemplateAsync(int templateId, CancellationToken cancellationToken)
        {
            return await trainingTemplateRepository.DeleteTrainingTemplateAsync(templateId, cancellationToken);
        }

        public async Task<TrainingTemplateResponseDto?> UpdateTrainingTemplateAsync(int id, CreateTrainingTemplateDto dto, CancellationToken cancellationToken)
        {
            // buscar la plantilla existente
            var dbTemplate = await trainingTemplateRepository.GetTrainingTemplateByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException($"Training template with ID {id} not found.");

            // Borrar los intervalos existentes (Es más sencillo para el update)
            await trainingTemplateRepository.DeleteAllAsociatedIntervalsAsync(id, cancellationToken);

            var newIntervals = dto.Intervals.Select((intervalDto, index) => new TrainingInterval
            {
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
                OrderIndex = intervalDto.OrderIndex,
                TrainingTemplateId = dbTemplate.Id  // Asignar FK
            }).ToList();

            // Agregar intervalos nuevos
            await trainingTemplateRepository.AddTrainingIntervalsToTemplateAsync(newIntervals, cancellationToken);

            // recargar la plantilla desde la base de datos para reflejar los cambios
            dbTemplate = await trainingTemplateRepository.GetTrainingTemplateByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException($"Training template with ID {id} not found after adding intervals.");

            // Actualizar los campos
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
            dbTemplate.Difficulty = dto.Difficulty;
            dbTemplate.Tags = dto.Tags.ToArray() ?? [];
            dbTemplate.UpdatedAt = DateTime.UtcNow;

            // Actualizar la plantilla en el repositorio
            await trainingTemplateRepository.UpdateTrainingTemplateAsync(dbTemplate, cancellationToken);
            
            // Recuperar la plantilla actualizada con sus intervalos
            var updatedTemplate = await trainingTemplateRepository.GetTrainingTemplateByIdAsync(dbTemplate.Id, cancellationToken);

            if (updatedTemplate == null)
                return null;

            // Mapear a DTO de respuesta
            var responseDto = MapToResponseDto(updatedTemplate);
            return responseDto;
        }
        public async Task<TrainingTemplateResponseDto?> ToggleFavoriteTemplateAsync(int id, CancellationToken cancellationToken)
        {
            // buscar la plantilla existente
            var dbTemplate = await trainingTemplateRepository.GetTrainingTemplateByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException($"Training template with ID {id} not found.");

            // Invertir el valor de favorite
            dbTemplate.IsFavorite = !dbTemplate.IsFavorite;
            dbTemplate.UpdatedAt = DateTime.UtcNow;

            await trainingTemplateRepository.UpdateTrainingTemplateAsync(dbTemplate, cancellationToken);

            var responseDto = MapToResponseDto(dbTemplate);
            return responseDto;
        }

        // <summary>
        /// Mapea una entidad TrainingTemplate a su DTO de respuesta
        /// </summary>
        private TrainingTemplateResponseDto MapToResponseDto(TrainingTemplate template)
        {
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
                Tags = template.Tags ?? Array.Empty<string>(),
                Intervals = template.Intervals
                    .OrderBy(i => i.OrderIndex)
                    .Select(i => new TrainingIntervalResponseDto
                    {
                        Id = i.Id,
                        Type = i.Type,
                        Repetitions = i.Repetitions,
                        Distance = i.Distance,
                        TargetTime = i.TargetTime,
                        RecoveryTime = i.RecoveryTime,
                        PaceType = i.PaceType,
                        Pace = i.Pace,
                        Vo2MaxPercentage = i.Vo2MaxPercentage,
                        Description = i.Description,
                        Intensity = i.Intensity,
                        TrainingMode = i.TrainingMode,
                        Duration = i.Duration,
                        TargetSpeed = i.TargetSpeed,
                        OrderIndex = i.OrderIndex
                    })
                    .ToList()
            };
        }

    }
}
