using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StriderWebApi.Dto.CompletedWorkout;
using StriderWebApi.Exceptions;
using StriderWebApi.Services.Interfaces;

namespace StriderWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CompletedWorkoutController(
        ICompletedWorkoutService completedWorkoutService,
        IJwtService jwtService) : ControllerBase
    {
        // GET: api/CompletedWorkout/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CompletedWorkoutResponseDto>> GetById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var workout = await completedWorkoutService.GetByIdAsync(id, cancellationToken);
                return Ok(workout);
            }
            catch (StriderWebApi.Exceptions.NotFoundException)
            {
                return NotFound();
            }
        }

        // POST: api/CompletedWorkout
        [HttpPost]
        public async Task<ActionResult<CompletedWorkoutResponseDto>> Create([FromBody] CreateCompletedWorkoutDto dto, CancellationToken cancellationToken)
        {
            var athleteId = jwtService.GetCurrentUserId();
            if (!athleteId.HasValue)
            {
                return Unauthorized("No se pudo determinar el usuario actual");
            }

            try
            {
                var workout = await completedWorkoutService.CreateAsync(dto, athleteId.Value, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = workout.Id }, workout);
            }
            catch (StriderWebApi.Exceptions.NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (StriderWebApi.Exceptions.UnauthorizedException ex)
            {
                return Forbid(ex.Message);
            }
            catch (System.ComponentModel.DataAnnotations.ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/CompletedWorkout/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<CompletedWorkoutResponseDto>> Update(int id, [FromBody] CreateCompletedWorkoutDto dto, CancellationToken cancellationToken)
        {
            var athleteId = jwtService.GetCurrentUserId();
            if (!athleteId.HasValue)
            {
                return Unauthorized("No se pudo determinar el usuario actual");
            }

            try
            {
                var workout = await completedWorkoutService.UpdateAsync(id, dto, athleteId.Value, cancellationToken);
                return Ok(workout);
            }
            catch (StriderWebApi.Exceptions.NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (StriderWebApi.Exceptions.UnauthorizedException ex)
            {
                return Forbid(ex.Message);
            }
            catch (System.ComponentModel.DataAnnotations.ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/CompletedWorkout/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var athleteId = jwtService.GetCurrentUserId();
            if (!athleteId.HasValue)
            {
                return Unauthorized("No se pudo determinar el usuario actual");
            }

            try
            {
                var result = await completedWorkoutService.DeleteAsync(id, athleteId.Value, cancellationToken);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (StriderWebApi.Exceptions.UnauthorizedException ex)
            {
                return Forbid(ex.Message);
            }
        }

        // GET: api/CompletedWorkout/athlete/mine
        [HttpGet("athlete/mine")]
        public async Task<ActionResult<IEnumerable<CompletedWorkoutResponseDto>>> GetMyCompletedWorkouts(
            [FromQuery] string? date = null,
            CancellationToken cancellationToken = default)
        {
            var athleteId = jwtService.GetCurrentUserId();
            if (!athleteId.HasValue)
            {
                return Unauthorized("No se pudo determinar el usuario actual");
            }

            DateTime? parsedDate = null;
            if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out var dateValue))
            {
                // Asegurar que la fecha esté en UTC para PostgreSQL
                parsedDate = dateValue.Kind == DateTimeKind.Utc 
                    ? dateValue 
                    : DateTime.SpecifyKind(dateValue, DateTimeKind.Utc);
            }

            var workouts = await completedWorkoutService.GetMyCompletedWorkoutsAsync(athleteId.Value, parsedDate, cancellationToken);
            return Ok(workouts);
        }

        // GET: api/CompletedWorkout/athlete/mine/range
        [HttpGet("athlete/mine/range")]
        public async Task<ActionResult<IEnumerable<CompletedWorkoutResponseDto>>> GetMyCompletedWorkoutsByDateRange(
            [FromQuery] string startDate,
            [FromQuery] string endDate,
            CancellationToken cancellationToken = default)
        {
            var athleteId = jwtService.GetCurrentUserId();
            if (!athleteId.HasValue)
            {
                return Unauthorized("No se pudo determinar el usuario actual");
            }

            if (!DateTime.TryParse(startDate, out var start) || !DateTime.TryParse(endDate, out var end))
            {
                return BadRequest("Las fechas deben estar en formato válido");
            }

            // Asegurar que las fechas estén en UTC para PostgreSQL
            var startUtc = start.Kind == DateTimeKind.Utc 
                ? start 
                : DateTime.SpecifyKind(start, DateTimeKind.Utc);
            var endUtc = end.Kind == DateTimeKind.Utc 
                ? end 
                : DateTime.SpecifyKind(end, DateTimeKind.Utc);

            var workouts = await completedWorkoutService.GetMyCompletedWorkoutsByDateRangeAsync(athleteId.Value, startUtc, endUtc, cancellationToken);
            return Ok(workouts);
        }

        // GET: api/CompletedWorkout/training-session-athlete/{trainingSessionAthleteId}
        [HttpGet("training-session-athlete/{trainingSessionAthleteId}")]
        public async Task<ActionResult<IEnumerable<CompletedWorkoutResponseDto>>> GetByTrainingSessionAthleteId(
            int trainingSessionAthleteId,
            CancellationToken cancellationToken = default)
        {
            var workouts = await completedWorkoutService.GetByTrainingSessionAthleteIdAsync(trainingSessionAthleteId, cancellationToken);
            return Ok(workouts);
        }

        // GET: api/CompletedWorkout/training-session-athlete/{trainingSessionAthleteId}/date
        [HttpGet("training-session-athlete/{trainingSessionAthleteId}/date")]
        public async Task<ActionResult<CompletedWorkoutResponseDto?>> GetByTrainingSessionAthleteIdAndDate(
            int trainingSessionAthleteId,
            [FromQuery] string date,
            CancellationToken cancellationToken = default)
        {
            if (!DateTime.TryParse(date, out var parsedDate))
            {
                return BadRequest("La fecha debe estar en formato válido");
            }

            // Asegurar que la fecha esté en UTC para PostgreSQL
            var dateUtc = parsedDate.Kind == DateTimeKind.Utc 
                ? parsedDate 
                : DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc);

            var workout = await completedWorkoutService.GetByTrainingSessionAthleteIdAndDateAsync(trainingSessionAthleteId, dateUtc, cancellationToken);
            // Retornar 200 con null en lugar de 404 para evitar errores en la consola del navegador
            return Ok(workout);
        }

        // GET: api/CompletedWorkout/coach/filtered
        [HttpGet("coach/filtered")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<CompletedWorkoutsGroupedByAthleteDto>>> GetForCoachWithFilters(
            [FromQuery] int? planningId = null,
            [FromQuery] int? trainingGroupId = null,
            [FromQuery] int? athleteId = null,
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] bool? hasFeedback = null,
            CancellationToken cancellationToken = default)
        {
            var coachId = jwtService.GetCurrentUserId();
            if (!coachId.HasValue)
            {
                return Unauthorized("No se pudo determinar el usuario actual");
            }

            DateTime? parsedStartDate = null;
            DateTime? parsedEndDate = null;

            if (!string.IsNullOrEmpty(startDate) && DateTime.TryParse(startDate, out var start))
            {
                parsedStartDate = start.Kind == DateTimeKind.Utc 
                    ? start 
                    : DateTime.SpecifyKind(start, DateTimeKind.Utc);
            }

            if (!string.IsNullOrEmpty(endDate) && DateTime.TryParse(endDate, out var end))
            {
                parsedEndDate = end.Kind == DateTimeKind.Utc 
                    ? end 
                    : DateTime.SpecifyKind(end, DateTimeKind.Utc);
            }

            var result = await completedWorkoutService.GetForCoachWithFiltersGroupedByAthleteAsync(
                coachId.Value,
                planningId,
                trainingGroupId,
                athleteId,
                parsedStartDate,
                parsedEndDate,
                hasFeedback,
                cancellationToken);

            return Ok(result);
        }

        // POST: api/CompletedWorkout/{id}/feedback
        [HttpPost("{id}/feedback")]
        [Authorize]
        public async Task<ActionResult<WorkoutFeedbackResponseDto>> SubmitWorkoutFeedback(
            int id,
            [FromBody] CreateWorkoutFeedbackDto dto,
            CancellationToken cancellationToken = default)
        {
            var coachId = jwtService.GetCurrentUserId();
            if (!coachId.HasValue)
            {
                return Unauthorized("No se pudo determinar el usuario actual");
            }

            try
            {
                var feedback = await completedWorkoutService.SubmitWorkoutFeedbackAsync(id, dto, coachId.Value, cancellationToken);
                return Ok(feedback);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedException ex)
            {
                return Forbid(ex.Message);
            }
            catch (System.ComponentModel.DataAnnotations.ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/CompletedWorkout/{id}/feedback
        [HttpPut("{id}/feedback")]
        [Authorize]
        public async Task<ActionResult<WorkoutFeedbackResponseDto>> UpdateWorkoutFeedback(
            int id,
            [FromBody] UpdateWorkoutFeedbackDto dto,
            CancellationToken cancellationToken = default)
        {
            var coachId = jwtService.GetCurrentUserId();
            if (!coachId.HasValue)
            {
                return Unauthorized("No se pudo determinar el usuario actual");
            }

            try
            {
                var feedback = await completedWorkoutService.UpdateWorkoutFeedbackAsync(id, dto, coachId.Value, cancellationToken);
                return Ok(feedback);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedException ex)
            {
                return Forbid(ex.Message);
            }
            catch (System.ComponentModel.DataAnnotations.ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

