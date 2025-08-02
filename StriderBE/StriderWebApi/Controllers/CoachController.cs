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
                Coach coach = await _coachService.GetCoachByIdAsync(coachId);

                CoachResponseDTO coachResponse = new(
                    coach.Name,
                    coach.TotalIndividualAthletes(),
                    coach.ActiveIndividualAthletes(),
                    coach.InactiveIndividualAthletes(),
                    coach.TotalWorkoutsCompletedByIndividualAthletes(),
                    [.. coach.Athletes.Select(a => new CoachResponseAthleteDTO(
                    a.Id,
                    a.Name,
                    DateTime.Today.Subtract(a.BirthDate).Days / 365,
                    a.Objectives,
                    a.TotalWorkoutsCompleted(),
                    a.GetLastWorkoutDate(),
                    a.IsActive(),
                    [.. a.GetActiveAilments().Select(a => new CoachResponseAthleteAilmentDTO(a.GetName(), a.Treatment))]
                    ))]
                );
                return Ok(coachResponse);
            }
            catch (CoachNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

        }



    }

    
}