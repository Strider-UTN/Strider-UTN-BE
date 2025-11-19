using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StriderWebApi.Services.Interfaces;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Dto.Garmin;
using System.Text.Json;

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

        [HttpPost("users/{userId}/login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GarminLoginResponseDto>> Login([FromRoute] int userId, [FromBody] GarminLoginRequestDto garminLoginRequestDto)
        {
            try
            {
                Athlete athlete = await _athleteService.GetAthleteByIdAsync(userId);
                var response = await _garminService.Login(athlete, garminLoginRequestDto);
                return Ok(response);
            } catch (Exception ex)
            {
                return Problem("An error occurred while logging in: " + ex.Message);
            }
        }

        [HttpPost("users/{userId}/workouts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<ActionResult<List<GarminWorkoutDto>>> GetWorkouts([FromRoute] int userId, [FromBody] GarminWorkoutRequestDto garminWorkoutRequestDto)
            {

            try
            {
                Athlete athlete = await _athleteService.GetAthleteByIdAsync(userId);
                var workouts = await _garminService.GetWorkouts(athlete, garminWorkoutRequestDto);
                return Ok(workouts);
            }
            catch (Exception ex)
            {
                return Problem("An error occurred while retrieving workouts: " + ex.Message);
            }

        }


    }
}

