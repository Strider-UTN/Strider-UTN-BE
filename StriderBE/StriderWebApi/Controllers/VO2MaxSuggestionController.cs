using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StriderWebApi.Dto.VO2MaxSuggestion;
using StriderWebApi.Services.Interfaces;

namespace StriderWebApi.Controllers
{
    /// <summary>
    /// Controlador para gestionar sugerencias de VO2Max
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VO2MaxSuggestionController : ControllerBase
    {
        private readonly IVO2MaxSuggestionService _service;
        private readonly ILogger<VO2MaxSuggestionController> _logger;

        public VO2MaxSuggestionController(
            IVO2MaxSuggestionService service,
            ILogger<VO2MaxSuggestionController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Crea una sugerencia de actualización de VO2Max (solo coaches)
        /// </summary>
        /// <param name="dto">Datos de la sugerencia</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>La sugerencia creada</returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(VO2MaxSuggestionResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VO2MaxSuggestionResponseDto>> CreateSuggestion(
            [FromBody] CreateVO2MaxSuggestionDto dto,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.CreateSuggestionAsync(dto, cancellationToken);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Intento de crear sugerencia sin autorización");
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Atleta no encontrado al crear sugerencia");
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Formato inválido de VO2Max");
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Error de operación al crear sugerencia");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear sugerencia");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Responde a una sugerencia de VO2Max (aceptar/rechazar) (solo atletas)
        /// </summary>
        /// <param name="dto">Datos de la respuesta</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>La sugerencia actualizada</returns>
        [HttpPost("respond")]
        [Authorize]
        [ProducesResponseType(typeof(VO2MaxSuggestionResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VO2MaxSuggestionResponseDto>> RespondToSuggestion(
            [FromBody] RespondToVO2MaxSuggestionDto dto,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.RespondToSuggestionAsync(dto, cancellationToken);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Intento de responder sugerencia sin autorización");
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Sugerencia no encontrada");
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Error de operación al responder sugerencia");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al responder sugerencia");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene las sugerencias pendientes del atleta actual (solo atletas)
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de sugerencias pendientes</returns>
        [HttpGet("pending")]
        [Authorize]
        [ProducesResponseType(typeof(List<VO2MaxSuggestionResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<VO2MaxSuggestionResponseDto>>> GetPendingSuggestions(
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.GetPendingSuggestionsAsync(cancellationToken);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Intento de obtener sugerencias sin autorización");
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener sugerencias pendientes");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene las sugerencias de VO2Max para un atleta específico (solo coaches)
        /// </summary>
        /// <param name="athleteId">ID del atleta</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de sugerencias del atleta</returns>
        [HttpGet("athlete/{athleteId}")]
        [Authorize]
        [ProducesResponseType(typeof(List<VO2MaxSuggestionResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<VO2MaxSuggestionResponseDto>>> GetSuggestionsByAthlete(
            int athleteId,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.GetSuggestionsByAthleteAsync(athleteId, cancellationToken);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Intento de obtener sugerencias sin autorización");
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener sugerencias del atleta");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}

