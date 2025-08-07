using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StriderWebApi.Dto.Athlete;
using StriderWebApi.Services.Interfaces;

namespace StriderWebApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AthleteController(IAthleteService athleteService) : ControllerBase
    {

        private readonly IAthleteService _athleteService = athleteService;

        [Authorize]
        [HttpGet("athletes/{athleteId}/feedback")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAthleteFeedback([FromRoute] int athleteId)
        {
            try
            {
                AthleteFeedbackResponseDTO athleteFeedback = await _athleteService.GetAthleteFeedback(athleteId);
                return Ok(athleteFeedback);
            }
            catch (AthleteNotFoundException e)
            {
                return NotFound("Athlete not found: " + e.Message);
            }
            catch (Exception e)
            {
                return Problem("An error occurred while retrieving athlete feedback: " + e.Message);
            }
        }
    }

    
    

}
