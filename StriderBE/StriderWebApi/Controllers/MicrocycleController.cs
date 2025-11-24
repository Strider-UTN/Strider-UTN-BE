using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StriderWebApi.Dto.Microcycle;
using StriderWebApi.Services.Interfaces;

namespace StriderWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MicrocycleController(
        IMicrocycleService microcycleService,
        IJwtService jwtService) : ControllerBase
    {

        // GET: api/Microcycle/mesocycle/{mesocycleId}
        [HttpGet("mesocycle/{mesocycleId}")]
        public async Task<ActionResult<IEnumerable<MicrocycleResponseDto>>> GetByMesocycleId(int mesocycleId, CancellationToken cancellationToken)
        {
            var microcycles = await microcycleService.GetByMesocycleIdAsync(mesocycleId, cancellationToken);
            return Ok(microcycles);
        }

        // GET: api/Microcycle/period/{periodId}
        [HttpGet("period/{periodId}")]
        public async Task<ActionResult<IEnumerable<MicrocycleResponseDto>>> GetByPeriodId(int periodId, CancellationToken cancellationToken)
        {
            var microcycles = await microcycleService.GetByPeriodIdAsync(periodId, cancellationToken);
            return Ok(microcycles);
        }

        // GET: api/Microcycle/planning/{planningId}
        [HttpGet("planning/{planningId}")]
        public async Task<ActionResult<IEnumerable<MicrocycleResponseDto>>> GetByPlanningId(int planningId, CancellationToken cancellationToken)
        {
            var microcycles = await microcycleService.GetByPlanningIdAsync(planningId, cancellationToken);
            return Ok(microcycles);
        }

        // GET: api/Microcycle/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MicrocycleResponseDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var microcycle = await microcycleService.GetByIdAsync(id, cancellationToken);
            if (microcycle == null) return NotFound();
            return Ok(microcycle);
        }

        // PUT: api/Microcycle/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<MicrocycleResponseDto>> Update(int id, [FromBody] UpdateMicrocycleDto dto, CancellationToken cancellationToken)
        {
            var coachId = jwtService.GetCurrentUserId();
            if (!coachId.HasValue) return Unauthorized();
            var microcycle = await microcycleService.UpdateAsync(id, dto, coachId.Value, cancellationToken);
            return Ok(microcycle);
        }

        // POST: api/Microcycle/{id}/recalculate-volume
        [HttpPost("{id}/recalculate-volume")]
        public async Task<ActionResult<decimal>> RecalculateVolume(int id, CancellationToken cancellationToken)
        {
            var volume = await microcycleService.RecalculateVolumeAsync(id, cancellationToken);
            return Ok(new { volume });
        }

        // DELETE: api/Microcycle/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var coachId = jwtService.GetCurrentUserId();
            if (!coachId.HasValue) return Unauthorized();
            var result = await microcycleService.DeleteAsync(id, coachId.Value, cancellationToken);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
