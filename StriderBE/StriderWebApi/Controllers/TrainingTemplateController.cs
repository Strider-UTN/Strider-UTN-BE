using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StriderWebApi.Dto;
using StriderWebApi.Dto.Trainings;
using StriderWebApi.Services.Interfaces;

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
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(TrainingTemplateResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TrainingTemplateResponseDto>> CreateTrainingTemplate(
            [FromBody] CreateTrainingTemplateDto dto,
            CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

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

                return CreatedAtAction(nameof(GetTrainingTemplate), new { id = responseDto.Id }, responseDto);
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
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(TrainingTemplateResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TrainingTemplateResponseDto>> GetTrainingTemplate(
            int id,
            CancellationToken cancellationToken)
        {
            var template = await trainingTemplateService.GetTrainingTemplateByIdAsync(id, cancellationToken);
            if (template == null)
            {
                return NotFound(new { message = $"No se encontró la plantilla con ID {id}" });
            }

            return Ok(template);
        }

        /// <summary>
        /// Obtiene todas las plantillas del usuario autenticado
        /// </summary>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(List<TrainingTemplateResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TrainingTemplateResponseDto>>> GetTrainingTemplate(
            CancellationToken cancellationToken)
        {
            try
            {
                var templates = await trainingTemplateService.GetAllTrainingTemplatesAsync(cancellationToken);
                return Ok(templates);
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
        /// Elimina una plantilla de entrenamiento
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteTrainingTemplate(int id, CancellationToken cancellationToken)
        {
            try
            {
                if (await trainingTemplateService.DeleteTrainingTemplateAsync(id, cancellationToken))
                {
                    return NoContent();
                }

                return NotFound(new { message = $"No se encontró la plantilla con ID {id}" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al eliminar la plantilla con id {id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Algo salió mal al eliminar la plantilla." });
            }
        }

        /// <summary>
        /// Actualiza una plantilla de entrenamiento
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(TrainingTemplateResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TrainingTemplateResponseDto>> UpdateTrainingTemplate(
            int id,
            [FromBody] CreateTrainingTemplateDto dto,
            CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = ValidateCreateTrainingTemplate(dto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new { errors = validationResult.Errors });
                }

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
        public async Task<ActionResult<TrainingTemplateResponseDto>> ToggleFavoriteTemplate(
            int id,
            CancellationToken cancellationToken)
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

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                errors.Add("El nombre de la plantilla es requerido");
            }
            else if (dto.Name.Length > 200)
            {
                errors.Add("El nombre de la plantilla no puede exceder 200 caracteres");
            }

            if (dto.Duration <= 0)
            {
                errors.Add("La duración debe ser mayor a 0");
            }

            if (dto.Series == null || !dto.Series.Any())
            {
                errors.Add("La plantilla debe incluir al menos una serie con intervalos");
            }
            else
            {
                for (int seriesIndex = 0; seriesIndex < dto.Series.Count; seriesIndex++)
                {
                    var series = dto.Series[seriesIndex];
                    if (series.Intervals == null || !series.Intervals.Any())
                    {
                        errors.Add($"La serie {seriesIndex + 1} debe contener al menos un intervalo");
                        continue;
                    }

                    for (int intervalIndex = 0; intervalIndex < series.Intervals.Count; intervalIndex++)
                    {
                        var interval = series.Intervals[intervalIndex];

                        if (interval.Repetitions <= 0)
                        {
                            errors.Add($"La serie {seriesIndex + 1}, intervalo {intervalIndex + 1} debe tener al menos 1 repetición");
                        }

                        if (interval.Distance < 0)
                        {
                            errors.Add($"La serie {seriesIndex + 1}, intervalo {intervalIndex + 1} tiene una distancia inválida");
                        }

                        if (string.IsNullOrWhiteSpace(interval.RecoveryTime))
                        {
                            errors.Add($"La serie {seriesIndex + 1}, intervalo {intervalIndex + 1} debe tener un tiempo de recuperación");
                        }
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
