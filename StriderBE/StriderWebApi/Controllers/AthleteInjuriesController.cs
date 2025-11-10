using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StriderWebApi.Dto.Injuries;
using StriderWebApi.Services.Interfaces;

namespace StriderWebApi.Controllers
{
    [ApiController]
    [Route("api/athlete/injuries")]
    [Authorize]
    public class AthleteInjuriesController(
        IAthleteInjuryService athleteInjuryService,
        IJwtService jwtService) : ControllerBase
    {
        private readonly IAthleteInjuryService _athleteInjuryService = athleteInjuryService;
        private readonly IJwtService _jwtService = jwtService;

        [HttpGet("mine")]
        [ProducesResponseType(typeof(IEnumerable<AthleteInjurySummaryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyInjuries(CancellationToken cancellationToken)
        {
            var athleteId = GetCurrentUserIdOrThrow();
            var injuries = await _athleteInjuryService.GetByAthleteAsync(athleteId, cancellationToken);
            return Ok(injuries);
        }

        [HttpGet("{injuryId:int}")]
        [ProducesResponseType(typeof(AthleteInjurySummaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetInjuryById(int injuryId, CancellationToken cancellationToken)
        {
            var athleteId = GetCurrentUserIdOrThrow();
            var injury = await _athleteInjuryService.GetByIdAsync(injuryId, athleteId, cancellationToken);
            if (injury == null)
            {
                return NotFound();
            }

            return Ok(injury);
        }

        [HttpPost]
        [ProducesResponseType(typeof(AthleteInjurySummaryDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateInjury([FromBody] CreateAthleteInjuryDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var athleteId = GetCurrentUserIdOrThrow();
            var created = await _athleteInjuryService.CreateAsync(athleteId, dto, cancellationToken);
            return CreatedAtAction(nameof(GetInjuryById), new { injuryId = created.Id }, created);
        }

        [HttpPut("{injuryId:int}")]
        [ProducesResponseType(typeof(AthleteInjurySummaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateInjury(int injuryId, [FromBody] UpdateAthleteInjuryDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var athleteId = GetCurrentUserIdOrThrow();

            try
            {
                var updated = await _athleteInjuryService.UpdateAsync(athleteId, injuryId, dto, cancellationToken);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        private int GetCurrentUserIdOrThrow()
        {
            var userId = _jwtService.GetCurrentUserId();
            if (!userId.HasValue)
            {
                throw new UnauthorizedAccessException("No se pudo determinar el usuario actual");
            }

            return userId.Value;
        }
    }
}
