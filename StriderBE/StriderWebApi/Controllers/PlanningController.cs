using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StriderWebApi.Dto.Planning;
using StriderWebApi.Dto.Trainings;
using StriderWebApi.Services;
using StriderWebApi.Services.Interfaces;

namespace StriderWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PlanningController(
        IPlanningService planningService,
        IJwtService jwtService) : ControllerBase
    {

        // GET: api/Planning
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlanningResponseDto>>> GetAll(CancellationToken cancellationToken)
        {
            var coachId = jwtService.GetCurrentUserId();
            if (!coachId.HasValue) return Unauthorized();
            var plannings = await planningService.GetByCoachIdAsync(coachId.Value, cancellationToken);
            return Ok(plannings);
        }

        // GET: api/Planning/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PlanningResponseDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var planning = await planningService.GetByIdAsync(id, cancellationToken);
            if (planning == null) return NotFound();
            return Ok(planning);
        }

        // POST: api/Planning
        [HttpPost]
        public async Task<ActionResult<PlanningResponseDto>> Create([FromBody] CreatePlanningDto dto, CancellationToken cancellationToken)
        {
            var coachId = jwtService.GetCurrentUserId();
            if (!coachId.HasValue) return Unauthorized();
            var planning = await planningService.CreateAsync(dto, coachId.Value, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = planning.Id }, planning);
        }

        // PUT: api/Planning/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<PlanningResponseDto>> Update(int id, [FromBody] UpdatePlanningDto dto, CancellationToken cancellationToken)
        {
            var coachId = jwtService.GetCurrentUserId();
            if (!coachId.HasValue) return Unauthorized();
            var planning = await planningService.UpdateAsync(id, dto, coachId.Value, cancellationToken);
            return Ok(planning);
        }

        // DELETE: api/Planning/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var coachId = jwtService.GetCurrentUserId();
            if (!coachId.HasValue) return Unauthorized();
            var result = await planningService.DeleteAsync(id, coachId.Value, cancellationToken);
            if (!result) return NotFound();
            return NoContent();
        }

        // POST: api/Planning/{id}/athletes
        [HttpPost("{id}/athletes")]
        public async Task<IActionResult> AssignAthletes(int id, [FromBody] AssignAthletesDto dto, CancellationToken cancellationToken)
        {
            var coachId = jwtService.GetCurrentUserId();
            if (!coachId.HasValue) return Unauthorized();
            var result = await planningService.AssignAthletesAsync(id, dto.AthleteIds, coachId.Value, cancellationToken);
            if (!result) return BadRequest();
            return Ok();
        }

        // POST: api/Planning/{id}/athletes/from-group/{groupId}
        [HttpPost("{id}/athletes/from-group/{groupId}")]
        public async Task<IActionResult> AssignAthletesFromGroup(int id, int groupId, CancellationToken cancellationToken)
        {
            var coachId = jwtService.GetCurrentUserId();
            if (!coachId.HasValue) return Unauthorized();
            var result = await planningService.AssignAthletesFromGroupAsync(id, groupId, coachId.Value, cancellationToken);
            if (!result) return BadRequest();
            return Ok();
        }

        // DELETE: api/Planning/{id}/athletes/{athleteId}
        [HttpDelete("{id}/athletes/{athleteId}")]
        public async Task<IActionResult> RemoveAthlete(int id, int athleteId, CancellationToken cancellationToken)
        {
            var coachId = jwtService.GetCurrentUserId();
            if (!coachId.HasValue) return Unauthorized();
            var result = await planningService.RemoveAthleteAsync(id, athleteId, coachId.Value, cancellationToken);
            if (!result) return NotFound();
            return NoContent();
        }

        // GET: api/Planning/athlete/{athleteId}
        [HttpGet("athlete/{athleteId}")]
        public async Task<ActionResult<IEnumerable<PlanningResponseDto>>> GetByAthleteId(int athleteId, CancellationToken cancellationToken)
        {
            var plannings = await planningService.GetByAthleteIdAsync(athleteId, cancellationToken);
            return Ok(plannings);
        }

        // GET: api/Planning/{id}/athletes
        [HttpGet("{id}/athletes")]
        public async Task<ActionResult<IEnumerable<PlanningAthleteResponseDto>>> GetAssignedAthletes(int id, CancellationToken cancellationToken)
        {
            var coachId = jwtService.GetCurrentUserId();
            if (!coachId.HasValue) return Unauthorized();

            var athletes = await planningService.GetAssignedAthletesAsync(id, coachId.Value, cancellationToken);
            return Ok(athletes);
        }
    }
}
