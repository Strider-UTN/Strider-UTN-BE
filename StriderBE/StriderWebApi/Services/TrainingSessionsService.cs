using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Domain.Enums;
using StriderWebApi.Dto.Trainings;
using StriderWebApi.Services.Interfaces;
using static StriderWebApi.Dto.Trainings.TrainingTemplateDto;

namespace StriderWebApi.Services
{
    public class TrainingSessionsService : ITrainingSessionsService
    {
        private readonly ITrainingSessionsRepository _repository;
        private readonly ILogger<TrainingSessionsService> _logger;

        public TrainingSessionsService(
            ITrainingSessionsRepository repository,
            ILogger<TrainingSessionsService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<TrainingSessionResponseDto> CreateTrainingSessionAsync(
            CreateTrainingSessionDto dto,
            int userId,
            CancellationToken cancellationToken = default)
        {
            // TODO: Validar que todos los atletas existen y pertenecen al usuario/coach

            // Crear la entidad TrainingSession - el enum ya viene parseado desde el DTO
            var trainingSession = new TrainingSession
            {
                Name = dto.Name,
                Description = dto.Description,
                Date = dto.Date,
                Category = dto.Category,
                Notes = dto.Notes,
                CreatedByUserId = userId,
                TemplateId = dto.TemplateId,
                CreatedAt = DateTime.UtcNow,
                Athletes = dto.AthleteIds.Select(athleteId => new TrainingSessionAthlete
                {
                    AthleteId = athleteId,
                    Status = SessionStatus.Pending,
                    AssignedAt = DateTime.UtcNow
                }).ToList(),
                Intervals = dto.Intervals.Select((intervalDto, index) => new TrainingInterval
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
                    CreatedAt = DateTime.UtcNow,
                    TrainingSessionId = null // Se establecerá después de crear la sesión
                }).ToList()
            };

            // Guardar la sesión
            var createdSession = await _repository.CreateTrainingSessionAsync(trainingSession, cancellationToken);

            // Establecer la referencia después de guardar
            foreach (var interval in createdSession.Intervals)
            {
                interval.TrainingSessionId = createdSession.Id;
            }
            await _repository.UpdateTrainingSessionAsync(createdSession, cancellationToken);

            // Recuperar la sesión completa con todas las relaciones
            var fullSession = await _repository.GetTrainingSessionByIdAsync(createdSession.Id, cancellationToken);

            if (fullSession == null)
                throw new InvalidOperationException("Error al recuperar la sesión creada");

            return MapToResponseDto(fullSession);
        }

        public async Task<TrainingSessionResponseDto?> GetTrainingSessionByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var session = await _repository.GetTrainingSessionByIdAsync(id, cancellationToken);
            return session != null ? MapToResponseDto(session) : null;
        }

        public async Task<List<TrainingSessionResponseDto>> GetAllTrainingSessionsAsync(int userId, CancellationToken cancellationToken = default)
        {
            var sessions = await _repository.GetAllTrainingSessionsAsync(userId, cancellationToken);
            return sessions.Select(MapToResponseDto).ToList();
        }

        public async Task<List<TrainingSessionResponseDto>> GetTrainingSessionsByDateAsync(DateTime date, int userId, CancellationToken cancellationToken = default)
        {
            var sessions = await _repository.GetTrainingSessionsByDateAsync(date, userId, cancellationToken);
            return sessions.Select(MapToResponseDto).ToList();
        }

        private TrainingSessionResponseDto MapToResponseDto(TrainingSession session)
        {
            return new TrainingSessionResponseDto
            {
                Id = session.Id,
                Name = session.Name,
                Description = session.Description,
                Date = session.Date,
                Category = session.Category,
                Notes = session.Notes,
                CreatedByUserId = session.CreatedByUserId,
                CreatedByName = session.CreatedBy?.Username ?? string.Empty,
                TemplateId = session.TemplateId,
                TemplateName = session.Template?.Name,
                CreatedAt = session.CreatedAt,
                UpdatedAt = session.UpdatedAt,
                Athletes = session.Athletes.Select(a => new TrainingSessionAthleteResponseDto
                {
                    Id = a.Id,
                    AthleteId = a.AthleteId,
                    AthleteName = a.Athlete?.Username ?? string.Empty,
                    Status = a.Status.ToString(),
                    CompletedAt = a.CompletedAt,
                    AssignedAt = a.AssignedAt
                }).ToList(),
                Intervals = session.Intervals.Select(i => new TrainingIntervalResponseDto
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
                }).ToList()
            };
        }
    }
}
