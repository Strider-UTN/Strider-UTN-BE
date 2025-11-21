using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StriderWebApi.Domain.Enums;
using StriderWebApi.Dto.UserCreation;
using StriderWebApi.Exceptions.AccountActivation;
using StriderWebApi.Exceptions.User;
using StriderWebApi.Services;
using StriderWebApi.Services.Interfaces;

namespace StriderWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserService userService, IJwtService jwtService) : ControllerBase
    {
        [HttpPost("Coach")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCoach([FromBody] CreateCoachDto dto)
        {
            try
            {
                await userService.CreateCoachAsync(dto);
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
                await userService.CreateAthleteAsync(dto);
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

        [HttpPost("Activate")]
        public async Task<IActionResult> ActivateAccount([FromBody] ActivateAccountDto dto)
        {
            try
            {
                await userService.ActivateAccountAsync(dto);
                return Ok("Cuenta activada exitosamente.");
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ActivationTokenInvalidOrExpiredException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Algo salió mal al activar la cuenta: " + ex.Message);
            }
        }

        /// <summary>
        /// Actualiza el tema preferido del usuario actual
        /// </summary>
        [HttpPatch("theme")]
        [Authorize]
        public async Task<IActionResult> UpdateTheme([FromBody] UpdateThemeDto dto)
        {
            try
            {
                var userId = jwtService.GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                // Convertir string a enum
                ThemePreference theme;
                if (dto.Theme.ToLowerInvariant() == "light")
                {
                    theme = ThemePreference.Light;
                }
                else if (dto.Theme.ToLowerInvariant() == "dark")
                {
                    theme = ThemePreference.Dark;
                }
                else
                {
                    return BadRequest(new { message = "El tema debe ser 'light' o 'dark'" });
                }

                var result = await userService.UpdateUserThemeAsync(userId.Value, theme);

                if (result)
                {
                    return Ok(new { message = "Tema actualizado exitosamente", theme = dto.Theme });
                }
                else
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar el tema", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene el perfil del usuario actual
        /// </summary>
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = jwtService.GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var profile = await userService.GetUserProfileAsync(userId.Value);

                if (profile == null)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                return Ok(profile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el perfil", error = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza el perfil del usuario actual
        /// </summary>
        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDto dto)
        {
            try
            {
                var userId = jwtService.GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var result = await userService.UpdateUserProfileAsync(userId.Value, dto);

                if (result)
                {
                    return Ok(new { message = "Perfil actualizado exitosamente" });
                }
                else
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar el perfil", error = ex.Message });
            }
        }
    }
}
