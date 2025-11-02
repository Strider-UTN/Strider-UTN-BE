using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StriderWebApi.Services.Interfaces;
using StriderWebApi.Model;

namespace StriderWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GarminController : ControllerBase
    {

        private readonly IGarminService _garminService;
        private readonly IAthleteService _athleteService;

        public GarminController(IGarminService garminService, IAthleteService athleteService)
        {
            _garminService = garminService;
            _athleteService = athleteService;
        }

        [HttpGet("users/{userId}/workouts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Workout>>> GetWorkouts([FromRoute] int userId, [FromQuery] DateTime start, [FromQuery] DateTime end, [FromQuery] string garminName, [FromQuery] string garminPassword)
        {

            try
            {
                Athlete athlete = await _athleteService.GetAthleteByIdAsync(userId);
                var workouts = await _garminService.GetWorkouts(athlete, start, end, garminName, garminPassword);
                athlete.AddWorkouts(workouts);
                await _athleteService.UpdateAthlete(athlete);
                return Ok(workouts);
            }
            catch (Exception ex)
            {
                return Problem("An error occurred while retrieving workouts: " + ex.Message);
            }
        }

        [HttpDelete("users/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteUser([FromRoute] int userId){
            try
            {
                Athlete athlete = await _athleteService.GetAthleteByIdAsync(userId);
                await _garminService.DeleteUser(athlete);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem("An error occurred while deleting user: " + ex.Message);
            }
        }

    }
}

