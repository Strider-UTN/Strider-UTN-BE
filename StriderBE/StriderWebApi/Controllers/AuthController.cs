using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using StriderWebApi.Dto.Login;
using StriderWebApi.Services.Interfaces;

namespace StriderWebApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService ?? throw new ArgumentNullException(nameof(authService));

        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            try
            {
                var token = await _authService.HandleLoginAsync(login.Email, login.Password, login.UserType);
                return Ok(new LoginResponseDto { Token = token });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your login request.", details = ex.Message });
            }
        }

        [HttpPost("Google-Login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken);

                var token = await _authService.HandleGoogleLoginAsync(payload, dto.UserType);

                return Ok(new LoginResponseDto { Token = token });
            }
            catch (InvalidJwtException ex)
            {
                return Unauthorized(new { message = "Invalid Google token", detail = ex.Message });
            }
            catch (Exception ex)
            {
                return Problem("Algo salió mal al ingresar al sistema con Google", ex.Message);
            }
        }

        [HttpPost("forgot-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            try
            {
                await _authService.ForgotPasswordAsync(dto.Email, dto.UserType);
                return Ok(new { message = "Se envió un correo de recuperación" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al procesar la solicitud de recuperación", details = ex.Message });
            }
        }

        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            try
            {
                await _authService.ResetPasswordAsync(dto.ResetToken, dto.NewPassword);
                return Ok(new { message = "Contraseña actualizada exitosamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al restablecer la contraseña", details = ex.Message });
            }
        }
    }
}
