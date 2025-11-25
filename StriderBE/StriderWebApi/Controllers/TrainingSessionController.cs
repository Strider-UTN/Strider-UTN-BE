using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StriderWebApi.Dto.Trainings;
using StriderWebApi.Services;
using StriderWebApi.Services.Interfaces;
using System.Security.Claims;

namespace StriderWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TrainingSessionController(
        ITrainingSessionService trainingSessionService,
        IJwtService jwtService) : ControllerBase
    {

        // GET: api/TrainingSession/planning/{planningId}
        [HttpGet("planning/{planningId}")]
        public async Task<ActionResult<IEnumerable<TrainingSessionResponseDto>>> GetByPlanningId(int planningId, CancellationToken cancellationToken)
        {
            var sessions = await trainingSessionService.GetByPlanningIdAsync(planningId, cancellationToken);
            return Ok(sessions);
        }

        // GET: api/TrainingSession/microcycle/{microcycleId}
        [HttpGet("microcycle/{microcycleId}")]
        public async Task<ActionResult<IEnumerable<TrainingSessionResponseDto>>> GetByMicrocycleId(int microcycleId, CancellationToken cancellationToken)
        {
            var sessions = await trainingSessionService.GetByMicrocycleIdAsync(microcycleId, cancellationToken);
            return Ok(sessions);
        }

        // GET: api/TrainingSession/mesocycle/{mesocycleId}
        [HttpGet("mesocycle/{mesocycleId}")]
        public async Task<ActionResult<IEnumerable<TrainingSessionResponseDto>>> GetByMesocycleId(int mesocycleId, CancellationToken cancellationToken)
        {
            var sessions = await trainingSessionService.GetByMesocycleIdAsync(mesocycleId, cancellationToken);
            return Ok(sessions);
        }

        // GET: api/TrainingSession/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TrainingSessionResponseDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var session = await trainingSessionService.GetByIdAsync(id, cancellationToken);
            if (session == null) return NotFound();
            return Ok(session);
        }

        // POST: api/TrainingSession
        // IMPORTANTE: Este endpoint identifica automáticamente el microciclo basado en PlanningId + Date
        [HttpPost]
        public async Task<ActionResult<TrainingSessionResponseDto>> Create([FromBody] CreateTrainingSessionDto dto, CancellationToken cancellationToken)
        {
            var coachId = jwtService.GetCurrentUserId();
            if (!coachId.HasValue) return Unauthorized();
            var session = await trainingSessionService.CreateWithAutoMicrocycleDetectionAsync(dto, coachId.Value, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = session.Id }, session);
        }

        // PUT: api/TrainingSession/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<TrainingSessionResponseDto>> Update(int id, [FromBody] UpdateTrainingSessionDto dto, CancellationToken cancellationToken)
        {
            var coachId = jwtService.GetCurrentUserId();
            if (!coachId.HasValue) return Unauthorized();
            var session = await trainingSessionService.UpdateAsync(id, dto, coachId.Value, cancellationToken);
            return Ok(session);
        }

        // DELETE: api/TrainingSession/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var coachId = jwtService.GetCurrentUserId();
            if (!coachId.HasValue) return Unauthorized();
            var result = await trainingSessionService.DeleteAsync(id, coachId.Value, cancellationToken);
            if (!result) return NotFound();
            return NoContent();
        }

        // GET: api/TrainingSession/athlete/{athleteId}
        [HttpGet("athlete/{athleteId}")]
        public async Task<ActionResult<IEnumerable<TrainingSessionResponseDto>>> GetByAthleteId(
            int athleteId,
            [FromQuery] int? planningId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            CancellationToken cancellationToken = default)
        {
            var sessions = await trainingSessionService.GetByAthleteIdAsync(athleteId, planningId, startDate, endDate, cancellationToken);
            return Ok(sessions);
        }

        // GET: api/TrainingSession/athlete/mine
        [HttpGet("athlete/mine")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<TrainingSessionResponseDto>>> GetMyTrainingSessions(
            [FromQuery] string? date = null,
            CancellationToken cancellationToken = default)
        {
            var userId = jwtService.GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("No se pudo determinar el usuario actual");
            }

            // En este sistema, el userId del JWT es el mismo que el athleteId porque Athlete hereda de User
            var athleteId = userId.Value;

            DateTime? dateFilter = null;
            if (!string.IsNullOrEmpty(date))
            {
                if (DateTime.TryParse(date, out var parsedDate))
                {
                    dateFilter = parsedDate;
                }
                else
                {
                    return BadRequest("Formato de fecha inválido. Use YYYY-MM-DD");
                }
            }

            var sessions = await trainingSessionService.GetMyTrainingSessionsAsync(athleteId, dateFilter, cancellationToken);
            return Ok(sessions);
        }
    }
}
