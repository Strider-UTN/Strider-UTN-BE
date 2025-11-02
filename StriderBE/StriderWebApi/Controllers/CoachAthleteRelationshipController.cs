using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StriderWebApi.Dto.Invitation;
using StriderWebApi.Services.Interfaces;

namespace StriderWebApi.Controllers
{
    /// <summary>
    /// Controlador para gestionar relaciones entre coaches y atletas
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CoachAthleteRelationshipsController : ControllerBase
    {
        private readonly ICoachAthleteRelationshipService _service;
        private readonly ILogger<CoachAthleteRelationshipsController> _logger;

        public CoachAthleteRelationshipsController(
            ICoachAthleteRelationshipService service,
            ILogger<CoachAthleteRelationshipsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Invita a un atleta por email (solo coaches)
        /// </summary>
        /// <param name="dto">Datos de la invitación</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>La relación creada</returns>
        /// <response code="200">Invitación creada exitosamente</response>
        /// <response code="400">Error en la solicitud</response>
        /// <response code="401">No autorizado</response>
        /// <response code="404">Atleta no encontrado</response>
        /// <response code="500">Error del servidor</response>
        [HttpPost("invite")]
        [Authorize]
        [ProducesResponseType(typeof(CoachAthleteRelationshipResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CoachAthleteRelationshipResponseDto>> InviteAthlete(
            [FromBody] InviteAthleteDto dto,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.InviteAthleteAsync(dto, cancellationToken);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Intento de invitar atleta sin autorización");
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Atleta no encontrado al invitar");
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Error de operación al invitar atleta");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al invitar atleta");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Responde a una invitación (aceptar/rechazar)
        /// </summary>
        /// <param name="dto">Datos de la respuesta</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>La relación actualizada</returns>
        /// <response code="200">Invitación respondida exitosamente</response>
        /// <response code="400">Error en la solicitud</response>
        /// <response code="401">No autorizado</response>
        /// <response code="404">Relación no encontrada</response>
        /// <response code="500">Error del servidor</response>
        [HttpPost("respond")]
        [Authorize]
        [ProducesResponseType(typeof(CoachAthleteRelationshipResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CoachAthleteRelationshipResponseDto>> RespondToInvitation(
            [FromBody] RespondToInvitationDto dto,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.RespondToInvitationAsync(dto, cancellationToken);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Intento de responder invitación sin autorización");
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Relación no encontrada al responder invitación");
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Error de operación al responder invitación");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al responder invitación");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene las invitaciones pendientes del atleta actual
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de invitaciones pendientes</returns>
        /// <response code="200">Lista de invitaciones pendientes</response>
        /// <response code="401">No autorizado</response>
        /// <response code="500">Error del servidor</response>
        [HttpGet("pending-invitations")]
        [Authorize]
        [ProducesResponseType(typeof(List<CoachAthleteRelationshipResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CoachAthleteRelationshipResponseDto>>> GetPendingInvitations(
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.GetPendingInvitationsAsync(cancellationToken);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Intento de obtener invitaciones pendientes sin autorización");
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener invitaciones pendientes");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene los coaches del atleta actual
        /// </summary>
        /// <param name="status">Filtro por estado (opcional)</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de coaches</returns>
        /// <response code="200">Lista de coaches</response>
        /// <response code="401">No autorizado</response>
        /// <response code="500">Error del servidor</response>
        [HttpGet("my-coaches")]
        [Authorize]
        [ProducesResponseType(typeof(List<CoachResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CoachResponseDto>>> GetMyCoaches(
            [FromQuery] string? status = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _service.GetMyCoachesAsync(status, cancellationToken);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Intento de obtener coaches sin autorización");
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener coaches");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene los atletas del coach actual
        /// </summary>
        /// <param name="status">Filtro por estado (opcional)</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de atletas</returns>
        /// <response code="200">Lista de atletas</response>
        /// <response code="401">No autorizado</response>
        /// <response code="500">Error del servidor</response>
        [HttpGet("my-athletes")]
        [Authorize]
        [ProducesResponseType(typeof(List<AthleteResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<AthleteResponseDto>>> GetMyAthletes(
            [FromQuery] string? status = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _service.GetMyAthletesAsync(status, cancellationToken);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Intento de obtener atletas sin autorización");
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener atletas");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Elimina una relación
        /// </summary>
        /// <param name="id">ID de la relación</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>True si se eliminó correctamente</returns>
        /// <response code="200">Relación eliminada exitosamente</response>
        /// <response code="400">Error en la solicitud</response>
        /// <response code="401">No autorizado</response>
        /// <response code="404">Relación no encontrada</response>
        /// <response code="500">Error del servidor</response>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RemoveRelationship(
            int id,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.RemoveRelationshipAsync(id, cancellationToken);
                if (result)
                {
                    return Ok(new { message = "Relación eliminada exitosamente" });
                }
                return NotFound(new { message = $"No se encontró la relación con ID {id}" });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Intento de eliminar relación sin autorización");
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Relación no encontrada al eliminar");
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Error de operación al eliminar relación");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar relación");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}
