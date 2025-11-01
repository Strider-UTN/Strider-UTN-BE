using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StriderWebApi.Dto.Trainings;
using StriderWebApi.Services.Interfaces;
using System.Security.Claims;

namespace StriderWebApi.Controllers
{
    // TrainingSessionsController.cs
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TrainingSessionsController : ControllerBase
    {
        private readonly ITrainingSessionsService _trainingSessionService;
        private readonly ILogger<TrainingSessionsController> _logger;

        public TrainingSessionsController(
            ITrainingSessionsService trainingSessionService,
            ILogger<TrainingSessionsController> logger)
        {
            _trainingSessionService = trainingSessionService;
            _logger = logger;
        }

        /// <summary>
        /// Crea una nueva sesión de entrenamiento
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(TrainingSessionResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TrainingSessionResponseDto>> CreateTrainingSession(
            [FromBody] CreateTrainingSessionDto dto,
            CancellationToken cancellationToken)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                if (userId == 0)
                    return Unauthorized(new { message = "Usuario no autenticado" });

                var session = await _trainingSessionService.CreateTrainingSessionAsync(dto, userId, cancellationToken);

                return CreatedAtAction(
                    nameof(GetTrainingSessionById),
                    new { id = session.Id },
                    session
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear sesión de entrenamiento");
                return StatusCode(500, new { message = "Error al crear la sesión de entrenamiento" });
            }
        }

        /// <summary>
        /// Obtiene una sesión por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TrainingSessionResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TrainingSessionResponseDto>> GetTrainingSessionById(
            int id,
            CancellationToken cancellationToken)
        {
            try
            {
                var session = await _trainingSessionService.GetTrainingSessionByIdAsync(id, cancellationToken);

                if (session == null)
                    return NotFound(new { message = $"Sesión con ID {id} no encontrada" });

                return Ok(session);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener sesión con ID {SessionId}", id);
                return StatusCode(500, new { message = "Error al obtener la sesión" });
            }
        }

        /// <summary>
        /// Obtiene todas las sesiones del usuario autenticado
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<TrainingSessionResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<TrainingSessionResponseDto>>> GetAllTrainingSessions(
            CancellationToken cancellationToken)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                if (userId == 0)
                    return Unauthorized(new { message = "Usuario no autenticado" });

                var sessions = await _trainingSessionService.GetAllTrainingSessionsAsync(userId, cancellationToken);
                return Ok(sessions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener sesiones de entrenamiento");
                return StatusCode(500, new { message = "Error al obtener las sesiones" });
            }
        }

        /// <summary>
        /// Obtiene todas las sesiones de una fecha específica
        /// </summary>
        [HttpGet("date/{date}")]
        [ProducesResponseType(typeof(List<TrainingSessionResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<TrainingSessionResponseDto>>> GetTrainingSessionsByDate(
            DateTime date,
            CancellationToken cancellationToken)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                if (userId == 0)
                    return Unauthorized(new { message = "Usuario no autenticado" });

                var sessions = await _trainingSessionService.GetTrainingSessionsByDateAsync(date, userId, cancellationToken);
                return Ok(sessions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener sesiones por fecha");
                return StatusCode(500, new { message = "Error al obtener las sesiones" });
            }
        }
    }
}
