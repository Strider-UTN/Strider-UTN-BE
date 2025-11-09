using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using StriderWebApi.Dto.Injuries;
using StriderWebApi.Services.Interfaces;

namespace StriderWebApi.Controllers
{
    [ApiController]
    [Route("api/coach/injuries")]
    [Authorize]
    public class CoachInjuriesController(IAthleteInjuryService athleteInjuryService, IJwtService jwtService) : ControllerBase
    {
        private readonly IAthleteInjuryService _athleteInjuryService = athleteInjuryService;
        private readonly IJwtService _jwtService = jwtService;

        [HttpGet("recent")]
        [ProducesResponseType(typeof(IEnumerable<CoachRecentInjuryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetRecentInjuries(CancellationToken cancellationToken = default)
        {
            var coachId = _jwtService.GetCurrentUserId();
            if (!coachId.HasValue)
            {
                return Unauthorized();
            }

            var injuries = await _athleteInjuryService.GetRecentInjuriesForCoachAsync(coachId.Value, cancellationToken);
            return Ok(injuries);
        }
    }
}
