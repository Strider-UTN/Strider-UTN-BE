using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StriderWebApi.Dto.Mesocycle;
using StriderWebApi.Exceptions;
using StriderWebApi.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace StriderWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MesocycleController(
        IMesocycleService mesocycleService,
        IJwtService jwtService) : ControllerBase
    {

        // GET: api/Mesocycle/planning/{planningId}
        [HttpGet("planning/{planningId}")]
        public async Task<ActionResult<IEnumerable<MesocycleResponseDto>>> GetByPlanningId(int planningId, CancellationToken cancellationToken)
        {
            var mesocycles = await mesocycleService.GetByPlanningIdAsync(planningId, cancellationToken);
            return Ok(mesocycles);
        }

        // GET: api/Mesocycle/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MesocycleResponseDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var mesocycle = await mesocycleService.GetByIdAsync(id, cancellationToken);
            if (mesocycle == null) return NotFound();
            return Ok(mesocycle);
        }

        // POST: api/Mesocycle/planning/{planningId}/period/{periodId}
        // IMPORTANTE: Este endpoint crea el mesociclo y genera automáticamente los microciclos
        [HttpPost("planning/{planningId}")]
        public async Task<ActionResult<MesocycleResponseDto>> CreateWithAutoMicrocycles(
            int planningId,
            [FromBody] CreateMesocycleDto dto,
            CancellationToken cancellationToken)
        {
            var coachId = jwtService.GetCurrentUserId();
            if (!coachId.HasValue) return Unauthorized();

            try
            {
                var mesocycle = await mesocycleService.CreateWithAutoMicrocyclesAsync(
                    dto, planningId, coachId.Value, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = mesocycle.Id }, mesocycle);
            }
            catch (ValidationException ex)
            {
                // Devolver BadRequest (400) con el mensaje de validación
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedException ex)
            {
                // Devolver Unauthorized (401) con el mensaje
                return Unauthorized(new { message = ex.Message });
            }
            catch (NotFoundException ex)
            {
                // Devolver NotFound (404) con el mensaje
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Devolver error 500 solo para errores inesperados
                return StatusCode(500, new { message = "Error interno del servidor al crear el mesociclo" });
            }
        }

        // PUT: api/Mesocycle/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<MesocycleResponseDto>> Update(int id, [FromBody] UpdateMesocycleDto dto, CancellationToken cancellationToken)
        {
            var coachId = jwtService.GetCurrentUserId();
            if (!coachId.HasValue) return Unauthorized();
            var mesocycle = await mesocycleService.UpdateAsync(id, dto, coachId.Value, cancellationToken);
            return Ok(mesocycle);
        }

        // DELETE: api/Mesocycle/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var coachId = jwtService.GetCurrentUserId();
            if (!coachId.HasValue) return Unauthorized();
            var result = await mesocycleService.DeleteAsync(id, coachId.Value, cancellationToken);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
