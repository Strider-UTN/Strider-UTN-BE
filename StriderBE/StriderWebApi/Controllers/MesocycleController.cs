using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StriderWebApi.Dto.Mesocycle;
using StriderWebApi.Services.Interfaces;

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
        [HttpPost("planning/{planningId}/period/{periodId}")]
        public async Task<ActionResult<MesocycleResponseDto>> CreateWithAutoMicrocycles(
            int planningId,
            int periodId,
            [FromBody] CreateMesocycleDto dto,
            CancellationToken cancellationToken)
        {
            var coachId = jwtService.GetCurrentUserId();
            if (!coachId.HasValue) return Unauthorized();
            var mesocycle = await mesocycleService.CreateWithAutoMicrocyclesAsync(
                dto, planningId, periodId, coachId.Value, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = mesocycle.Id }, mesocycle);
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
