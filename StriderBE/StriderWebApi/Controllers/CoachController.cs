using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StriderWebApi.Data.Repositories;
using StriderWebApi.Dto.Coach;
using StriderWebApi.Exceptions.Coach;
using StriderWebApi.Model;
using StriderWebApi.Services.Interfaces;

namespace StriderWebApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CoachController(ICoachService coachService) : ControllerBase
    {

        private ICoachService _coachService = coachService;

        [Authorize]
        [HttpGet("/{coachId}/athletes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetIndividualAthletes(int coachId)
        {
            try
            {
                CoachResponseDTO coachResponse = await _coachService.GetCoachIndividualAthletes(coachId);
                return Ok(coachResponse);
            }
            catch (CoachNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

        }

        [HttpPost("/Athlete/{athleteId}/Feedback/{workoutId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> PostFeedbackAsync([FromRoute] int athleteId, [FromRoute] int workoutId, [FromBody] CoachFeedbackDTO feedback)
        {
            try
            {
                await _coachService.PostWorkoutFeedbackAsync(athleteId, workoutId, feedback);
                return NoContent();
            }
            catch (AthleteNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

    }

}