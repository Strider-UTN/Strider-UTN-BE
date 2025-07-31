using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StriderWebApi.Domain.Enums;
using StriderWebApi.Dto.UserCreation;
using StriderWebApi.Exceptions.User;
using StriderWebApi.Services.Interfaces;

namespace StriderWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Coach")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCoach([FromBody] CreateCoachDto dto)
        {
            try
            {
                await _userService.CreateCoachAsync(dto);
                return CreatedAtAction(nameof(CreateCoach), new { username = dto.Username }, null);
            }
            catch (UserAlreadyExistsException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Algo salió mal al crear un nuevo Entrenador: " + ex.Message);
            }
        }

        [HttpPost("Athlete")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAthlete([FromBody] CreateAthleteDto dto)
        {
            try
            {
                await _userService.CreateAthleteAsync(dto);
                return CreatedAtAction(nameof(CreateAthlete), new { username = dto.Username }, null);
            }
            catch (UserAlreadyExistsException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Algo salió mal al crear un nuevo Atleta: " + ex.Message);
            }
        }
    }
}
