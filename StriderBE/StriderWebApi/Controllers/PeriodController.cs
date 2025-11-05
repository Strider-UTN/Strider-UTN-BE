using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StriderWebApi.Dto.Period;
using StriderWebApi.Services.Interfaces;

namespace StriderWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PeriodController(
        IPeriodService periodService,
        IJwtService jwtService) : ControllerBase
    {

        // GET: api/Period/planning/{planningId}
        [HttpGet("planning/{planningId}")]
        public async Task<ActionResult<IEnumerable<PeriodResponseDto>>> GetByPlanningId(int planningId, CancellationToken cancellationToken)
        {
            var periods = await periodService.GetByPlanningIdAsync(planningId, cancellationToken);
            return Ok(periods);
        }

        // GET: api/Period/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PeriodResponseDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var period = await periodService.GetByIdAsync(id, cancellationToken);
            if (period == null) return NotFound();
            return Ok(period);
        }

        // POST: api/Period/planning/{planningId}
        [HttpPost("planning/{planningId}")]
        public async Task<ActionResult<PeriodResponseDto>> Create(int planningId, [FromBody] CreatePeriodDto dto, CancellationToken cancellationToken)
        {
            var coachId = jwtService.GetCurrentUserId();
            if (!coachId.HasValue) return Unauthorized();
            var period = await periodService.CreateAsync(dto, planningId, coachId.Value, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = period.Id }, period);
        }

        // PUT: api/Period/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<PeriodResponseDto>> Update(int id, [FromBody] UpdatePeriodDto dto, CancellationToken cancellationToken)
        {
            var coachId = jwtService.GetCurrentUserId();
            if (!coachId.HasValue) return Unauthorized();
            var period = await periodService.UpdateAsync(id, dto, coachId.Value, cancellationToken);
            return Ok(period);
        }

        // DELETE: api/Period/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var coachId = jwtService.GetCurrentUserId();
            if (!coachId.HasValue) return Unauthorized();
            var result = await periodService.DeleteAsync(id, coachId.Value, cancellationToken);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
