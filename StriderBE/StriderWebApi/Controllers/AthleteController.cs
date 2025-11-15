using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StriderWebApi.Data.Repositories;
using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Dto.Athlete;
using StriderWebApi.Services.Interfaces;

namespace StriderWebApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AthleteController(IAthleteService athleteService, IJwtService jwtService, List<IAthleteAnalysisService> athleteAnalysisServices, ITrainingSessionsRepository trainingSessionAthleteRepository) : ControllerBase
    {

        private readonly IAthleteService _athleteService = athleteService;
        private readonly IJwtService _jwtService = jwtService;

        private readonly List<IAthleteAnalysisService> _athleteAnalysisServices = athleteAnalysisServices;

        private readonly ITrainingSessionsRepository _trainingSessionAthleteRepository = trainingSessionAthleteRepository;


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

        [Authorize]
        [HttpGet("me/status")]
        [ProducesResponseType(typeof(AthleteStatusResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCurrentAthleteStatus(CancellationToken cancellationToken)
        {
            var athleteId = _jwtService.GetCurrentUserId();
            if (!athleteId.HasValue)
            {
                return Unauthorized();
            }

            try
            {
                var isActive = await _athleteService.GetActiveStatusAsync(athleteId.Value, cancellationToken);
                return Ok(new AthleteStatusResponseDto { IsActive = isActive });
            }
            catch (AthleteNotFoundException)
            {
                return NotFound();
            }
        }

        [Authorize]
        [HttpPut("me/status")]
        [ProducesResponseType(typeof(AthleteStatusResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCurrentAthleteStatus([FromBody] UpdateAthleteStatusDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var athleteId = _jwtService.GetCurrentUserId();
            if (!athleteId.HasValue)
            {
                return Unauthorized();
            }

            try
            {
                await _athleteService.UpdateActiveStatusAsync(athleteId.Value, dto.IsActive, cancellationToken);
                return Ok(new AthleteStatusResponseDto { IsActive = dto.IsActive });
            }
            catch (AthleteNotFoundException)
            {
                return NotFound();
            }
        }


        [Authorize]
        [HttpGet("athlete/{athleteId}/analysis")]
        [ProducesResponseType(typeof(TrainingStatus), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<List<TrainingStatus>> GetAthleteAnalysis([FromRoute] int athleteId)
        {
            var athlete = await _athleteService.GetAthleteByIdAsync(athleteId);
            var trainingSessions = await _trainingSessionAthleteRepository.GetByAthleteIdAsync(athleteId);
            List<TrainingStatus> analysis = _athleteAnalysisServices.Select(service => service.Analyze(athlete, trainingSessions)).ToList();
            return analysis;
        }
        
    }
}
