using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Domain.Enums;
using StriderWebApi.Dto.VO2MaxSuggestion;
using StriderWebApi.Services.Interfaces;

namespace StriderWebApi.Services
{
    /// <summary>
    /// Servicio para gestionar sugerencias de VO2Max
    /// </summary>
    public class VO2MaxSuggestionService(
        IVO2MaxSuggestionRepository suggestionRepository,
        IUserRepository userRepository,
        IUserService userService,
        IJwtService jwtService,
        ILogger<VO2MaxSuggestionService> logger) : IVO2MaxSuggestionService
    {
        public async Task<VO2MaxSuggestionResponseDto> CreateSuggestionAsync(
            CreateVO2MaxSuggestionDto dto,
            CancellationToken cancellationToken = default)
        {
            // Obtener el coach actual desde el token JWT
            var currentUserId = jwtService.GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                throw new UnauthorizedAccessException("Usuario no autenticado");
            }

            var coach = await userRepository.GetUserByIdAsync(currentUserId.Value);
            if (coach == null)
            {
                throw new KeyNotFoundException("Coach no encontrado");
            }

            // Verificar que el usuario actual es un coach
            if (coach.UserType != UserTypeEnum.Coach)
            {
                throw new UnauthorizedAccessException("Solo los coaches pueden sugerir actualizaciones de VO2Max");
            }

            // Verificar que el atleta existe
            var athlete = await userRepository.GetUserByIdAsync(dto.AthleteId);
            if (athlete == null)
            {
                throw new KeyNotFoundException($"No se encontró un atleta con ID {dto.AthleteId}");
            }

            // Verificar que el usuario es un atleta
            if (athlete.UserType != UserTypeEnum.Athlete)
            {
                throw new InvalidOperationException($"El usuario con ID {dto.AthleteId} no es un atleta");
            }

            // Validar formato de VO2Max (mm:ss)
            if (!IsValidVO2MaxFormat(dto.SuggestedVO2Max))
            {
                throw new ArgumentException("El formato de VO2Max debe ser mm:ss (ejemplo: 03:30)");
            }

            // Crear la sugerencia
            var suggestion = new VO2MaxSuggestion
            {
                CoachId = coach.Id,
                AthleteId = athlete.Id,
                SuggestedVO2Max = dto.SuggestedVO2Max,
                Message = dto.Message,
                Status = VO2MaxSuggestionStatus.Pending,
                SuggestedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            var createdSuggestion = await suggestionRepository.CreateSuggestionAsync(suggestion, cancellationToken);

            // Cargar la sugerencia con los usuarios incluidos
            var suggestionWithUsers = await suggestionRepository.GetSuggestionByIdAsync(
                createdSuggestion.Id,
                cancellationToken);

            if (suggestionWithUsers == null)
            {
                throw new InvalidOperationException("Error al crear la sugerencia");
            }

            logger.LogInformation(
                "Sugerencia de VO2Max creada por coach {CoachId} para atleta {AthleteId}",
                coach.Id,
                athlete.Id);

            return MapToResponseDto(suggestionWithUsers);
        }

        public async Task<VO2MaxSuggestionResponseDto> RespondToSuggestionAsync(
            RespondToVO2MaxSuggestionDto dto,
            CancellationToken cancellationToken = default)
        {
            // Obtener el atleta actual desde el token JWT
            var currentUserId = jwtService.GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                throw new UnauthorizedAccessException("Usuario no autenticado");
            }

            var athlete = await userRepository.GetUserByIdAsync(currentUserId.Value) ?? throw new KeyNotFoundException("Atleta no encontrado");

            // Verificar que el usuario actual es un atleta
            if (athlete.UserType != UserTypeEnum.Athlete)
            {
                throw new UnauthorizedAccessException("Solo los atletas pueden responder sugerencias");
            }

            // Obtener la sugerencia
            var suggestion = await suggestionRepository.GetSuggestionByIdAsync(
                dto.SuggestionId,
                cancellationToken);

            if (suggestion == null)
            {
                throw new KeyNotFoundException($"No se encontró la sugerencia con ID {dto.SuggestionId}");
            }

            // Verificar que la sugerencia es para el atleta actual
            if (suggestion.AthleteId != athlete.Id)
            {
                throw new UnauthorizedAccessException("No tienes permiso para responder esta sugerencia");
            }

            // Verificar que la sugerencia está pendiente
            if (suggestion.Status != VO2MaxSuggestionStatus.Pending)
            {
                throw new InvalidOperationException("Esta sugerencia ya fue respondida");
            }

            // Actualizar el estado de la sugerencia
            suggestion.Status = dto.Accept ? VO2MaxSuggestionStatus.Accepted : VO2MaxSuggestionStatus.Rejected;
            suggestion.RespondedAt = DateTime.UtcNow;
            suggestion.UpdatedAt = DateTime.UtcNow;

            // Si se acepta, actualizar el VO2Max del atleta
            if (dto.Accept)
            {
                var updateDto = new Dto.UserCreation.UpdateUserProfileDto
                {
                    VO2Max = suggestion.SuggestedVO2Max
                };

                await userService.UpdateUserProfileAsync(athlete.Id, updateDto);

                logger.LogInformation(
                    "VO2Max actualizado para atleta {AthleteId} a {VO2Max}",
                    athlete.Id,
                    suggestion.SuggestedVO2Max);
            }

            var updatedSuggestion = await suggestionRepository.UpdateSuggestionAsync(suggestion, cancellationToken);

            // Cargar la sugerencia actualizada con los usuarios incluidos
            var suggestionWithUsers = await suggestionRepository.GetSuggestionByIdAsync(
                updatedSuggestion.Id,
                cancellationToken);

            if (suggestionWithUsers == null)
            {
                throw new InvalidOperationException("Error al actualizar la sugerencia");
            }

            logger.LogInformation(
                "Sugerencia de VO2Max {SuggestionId} {Status} por atleta {AthleteId}",
                dto.SuggestionId,
                dto.Accept ? "aceptada" : "rechazada",
                athlete.Id);

            return MapToResponseDto(suggestionWithUsers);
        }

        public async Task<List<VO2MaxSuggestionResponseDto>> GetPendingSuggestionsAsync(
            CancellationToken cancellationToken = default)
        {
            // Obtener el atleta actual desde el token JWT
            var currentUserId = jwtService.GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                throw new UnauthorizedAccessException("Usuario no autenticado");
            }

            var athlete = await userRepository.GetUserByIdAsync(currentUserId.Value);
            if (athlete == null)
            {
                throw new KeyNotFoundException("Atleta no encontrado");
            }

            // Verificar que el usuario actual es un atleta
            if (athlete.UserType != UserTypeEnum.Athlete)
            {
                throw new UnauthorizedAccessException("Solo los atletas pueden ver sugerencias pendientes");
            }

            var suggestions = await suggestionRepository.GetPendingSuggestionsByAthleteAsync(
                athlete.Id,
                cancellationToken);

            return suggestions.Select(MapToResponseDto).ToList();
        }

        public async Task<List<VO2MaxSuggestionResponseDto>> GetSuggestionsByAthleteAsync(
            int athleteId,
            CancellationToken cancellationToken = default)
        {
            // Obtener el coach actual desde el token JWT
            var currentUserId = jwtService.GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                throw new UnauthorizedAccessException("Usuario no autenticado");
            }

            var coach = await userRepository.GetUserByIdAsync(currentUserId.Value) ?? throw new KeyNotFoundException("Coach no encontrado");

            // Verificar que el usuario actual es un coach
            if (coach.UserType != UserTypeEnum.Coach)
            {
                throw new UnauthorizedAccessException("Solo los coaches pueden ver sugerencias de atletas");
            }

            var suggestions = await suggestionRepository.GetSuggestionsByAthleteAsync(
                athleteId,
                null,
                cancellationToken);

            return suggestions.Select(MapToResponseDto).ToList();
        }

        private static bool IsValidVO2MaxFormat(string vo2Max)
        {
            if (string.IsNullOrWhiteSpace(vo2Max))
                return false;

            // Formato esperado: mm:ss (ejemplo: 03:30)
            var parts = vo2Max.Split(':');
            if (parts.Length != 2)
                return false;

            if (!int.TryParse(parts[0], out var minutes) || minutes < 0 || minutes > 59)
                return false;

            if (!int.TryParse(parts[1], out var seconds) || seconds < 0 || seconds > 59)
                return false;

            return true;
        }

        private static VO2MaxSuggestionResponseDto MapToResponseDto(VO2MaxSuggestion suggestion)
        {
            return new VO2MaxSuggestionResponseDto
            {
                Id = suggestion.Id,
                CoachId = suggestion.CoachId,
                CoachName = suggestion.Coach.FullName,
                CoachEmail = suggestion.Coach.Email,
                AthleteId = suggestion.AthleteId,
                AthleteName = suggestion.Athlete.FullName,
                AthleteEmail = suggestion.Athlete.Email,
                SuggestedVO2Max = suggestion.SuggestedVO2Max,
                Message = suggestion.Message,
                Status = suggestion.Status.ToString(),
                SuggestedAt = suggestion.SuggestedAt,
                RespondedAt = suggestion.RespondedAt
            };
        }
    }
}

