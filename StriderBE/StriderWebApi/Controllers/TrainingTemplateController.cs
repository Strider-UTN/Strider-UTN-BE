using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using StriderWebApi.Dto;
using StriderWebApi.Services.Interfaces;
using static StriderWebApi.Dto.Trainings.TrainingTemplateDto;

namespace StriderWebApi.Controllers
{
    /// <summary>
    /// Controlador para gestionar plantillas de entrenamiento
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TrainingTemplatesController(
        ITrainingTemplateService trainingTemplateService,
        ILogger<TrainingTemplatesController> logger) : ControllerBase
    {

        /// <summary>
        /// Crea una nueva plantilla de entrenamiento
        /// </summary>
        /// <param name="dto">Datos de la plantilla a crear</param>
        /// <returns>La plantilla creada con su ID asignado</returns>
        /// <response code="201">Plantilla creada exitosamente</response>
        /// <response code="400">Datos inválidos</response>
        /// <response code="401">No Autorizado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost("")]
        [Authorize]
        [ProducesResponseType(typeof(TrainingTemplateResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TrainingTemplateResponseDto>> CreateTrainingTemplate(
            [FromBody] CreateTrainingTemplateDto dto, CancellationToken cancellationToken)
        {
            try
            {
                // Validar modelo
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validaciones adicionales
                var validationResult = ValidateCreateTrainingTemplate(dto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new { errors = validationResult.Errors });
                }

                var responseDto = await trainingTemplateService.CreateTrainingTemplateAsync(dto, cancellationToken);

                if (responseDto == null)
                {
                    logger.LogError("Error al recuperar la plantilla creada.");
                    return StatusCode(500, new { message = "Error al crear la plantilla" });
                }

                return CreatedAtAction(
                    nameof(GetTrainingTemplate),
                    new { id = responseDto.Id },
                    responseDto
                );
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Error al guardar la plantilla en la base de datos");
                return StatusCode(500, new { message = "Error al guardar la plantilla en la base de datos", details = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al crear la plantilla");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene una plantilla por su ID
        /// </summary>
        /// <param name="id">ID de la plantilla</param>
        /// <returns>La plantilla solicitada</returns>
        /// <response code="200">Plantilla encontrada</response>
        /// <response code="401">No Autorizado</response>
        /// <response code="404">Plantilla no encontrada</response>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(TrainingTemplateResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TrainingTemplateResponseDto>> GetTrainingTemplate(int id, CancellationToken cancellationToken)
        {
            var template = await trainingTemplateService.GetTrainingTemplateByIdAsync(id, cancellationToken);

            if (template == null)
            {
                return NotFound(new { message = $"No se encontró la plantilla con ID {id}" });
            }

            return Ok(template);
        }

        /// <summary>
        /// Obtiene todas las plantillas del usuario
        /// </summary>
        /// <returns>Las plantillas creadas por el usuario</returns>
        /// <response code="200">Plantillas encontradas</response>
        /// <response code="401">No Autorizado</response>
        [HttpGet("")]
        [Authorize]
        [ProducesResponseType(typeof(List<TrainingTemplateResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<TrainingTemplateResponseDto>>> GetTrainingTemplate(CancellationToken cancellationToken)
        {
            try
            {
                var template = await trainingTemplateService.GetAllTrainingTemplatesAsync(cancellationToken);
                return Ok(template);
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.LogError(ex, "Acceso no autorizado al obtener las plantillas");
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al obtener las plantillas");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Elmina la plantilla de entrenamiento por su ID
        /// </summary>
        /// <response code="204">Plantilla eliminada</response>
        /// <response code="401">No Autorizado</response>
        /// <response code="404">Plantilla no encontrada</response>
        /// <response code="500">Error del servidor</response>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteTrainingTemplate(int id, CancellationToken cancellationToken)
        {
            try
            {
                if( await trainingTemplateService.DeleteTrainingTemplateAsync(id, cancellationToken))
                    return NoContent();

                return NotFound( new { message = $"No se encontró la plantilla con ID {id}" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al eliminar la plantilla con id {id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Algo salió mal al eliminar la plantilla." });
            }
        }

        /// <summary>
        /// Actualiza una plantilla de entrenamiento por su ID
        /// </summary>
        /// <returns>La plantilla Actualizada</returns>
        /// <response code="200">Plantilla Actualizada</response>
        /// <response code="401">No Autorizado</response>
        /// <response code="404">Plantilla no encontrada</response>
        /// <response code="500">Error del servidor</response>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(TrainingTemplateResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TrainingTemplateResponseDto>> UpdateTrainingTemplate(int id, [FromBody] CreateTrainingTemplateDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var responseDto = await trainingTemplateService.UpdateTrainingTemplateAsync(id, dto, cancellationToken);

                if (responseDto == null)
                {
                    logger.LogError("Error al recuperar la plantilla actualizada.");
                    return StatusCode(500, new { message = "Error al actualizar la plantilla" });
                }

                return Ok(responseDto);
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogError(ex, "Plantilla con id {id} no encontrada para actualizar", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al actualizar la plantilla con id {id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Algo salió mal al actualizar la plantilla." });
            }
        }

        [HttpPatch("{id}/favorite")]
        [Authorize]
        [ProducesResponseType(typeof(TrainingTemplateResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TrainingTemplateResponseDto>> ToggleFavoriteTemplate(int id, CancellationToken cancellationToken)
        {
            try
            {
                var response = await trainingTemplateService.ToggleFavoriteTemplateAsync(id, cancellationToken);

                if (response == null)
                {
                    logger.LogError("Error al recuperar la plantilla actualizada.");
                    return StatusCode(500, new { message = "Error al actualizar la plantilla" });
                }

                return response;
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogError(ex, "Plantilla con id {id} no encontrada para actualizar", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al actualizar la plantilla con id {id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Algo salió mal al actualizar la plantilla." });
            }
        }


        /// <summary>
        /// Valida los datos de creación de plantilla
        /// </summary>
        private ValidationResult ValidateCreateTrainingTemplate(CreateTrainingTemplateDto dto)
        {
            var errors = new List<string>();

            // Validar nombre
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                errors.Add("El nombre de la plantilla es requerido");
            }
            else if (dto.Name.Length > 200)
            {
                errors.Add("El nombre de la plantilla no puede exceder 200 caracteres");
            }

            // Validar duración
            if (dto.Duration <= 0)
            {
                errors.Add("La duración debe ser mayor a 0");
            }

            // Validar intervalos
            if (dto.Intervals != null && dto.Intervals.Any())
            {
                for (int i = 0; i < dto.Intervals.Count; i++)
                {
                    var interval = dto.Intervals[i];

                    if (interval.Repetitions <= 0)
                    {
                        errors.Add($"El intervalo {i + 1} debe tener al menos 1 repetición");
                    }

                    if (interval.Distance < 0)
                    {
                        errors.Add($"El intervalo {i + 1} tiene una distancia inválida");
                    }

                    if (string.IsNullOrWhiteSpace(interval.RecoveryTime))
                    {
                        errors.Add($"El intervalo {i + 1} debe tener un tiempo de recuperación");
                    }
                }
            }

            return new ValidationResult
            {
                IsValid = !errors.Any(),
                Errors = errors
            };
        }
    }
}
