using StriderWebApi.Services.Interfaces;
using System.Security.Claims;

namespace StriderWebApi.Services
{
    /// <summary>
    /// Implementación del servicio para leer información del token JWT
    /// </summary>
    public class JwtService : IJwtService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<JwtService> _logger;

        public JwtService(
            IHttpContextAccessor httpContextAccessor,
            ILogger<JwtService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene el ID del usuario autenticado desde el token JWT
        /// Extrae el valor del claim ClaimTypes.NameIdentifier
        /// </summary>
        public int? GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
            {
                _logger.LogWarning("HttpContext o User no están disponibles");
                return null;
            }

            // Obtener el claim NameIdentifier que contiene el userId
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                _logger.LogWarning("No se pudo obtener el userId del token JWT");
                return null;
            }

            // Intentar parsear el userId a int
            if (int.TryParse(userIdClaim, out int parsedUserId))
            {
                return parsedUserId;
            }

            _logger.LogWarning("El userId del token no es un número válido: {UserIdClaim}", userIdClaim);
            return null;
        }

        /// <summary>
        /// Obtiene el nombre de usuario del token JWT
        /// Extrae el valor del claim "Name"
        /// </summary>
        public string? GetCurrentUserName()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
            {
                return null;
            }

            return user.FindFirst("Name")?.Value;
        }

        /// <summary>
        /// Obtiene el rol del usuario del token JWT
        /// Extrae el valor del claim "Role"
        /// </summary>
        public string? GetCurrentUserRole()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
            {
                return null;
            }

            return user.FindFirst("Role")?.Value;
        }
    }
}
