namespace StriderWebApi.Services.Interfaces
{
    /// <summary>
    /// Servicio para obtener información del token JWT del usuario autenticado
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Obtiene el ID del usuario autenticado desde el token JWT
        /// </summary>
        /// <returns>El ID del usuario o null si no está autenticado o el token es inválido</returns>
        int? GetCurrentUserId();

        /// <summary>
        /// Obtiene el nombre de usuario del token JWT
        /// </summary>
        /// <returns>El nombre de usuario o null si no está disponible</returns>
        string? GetCurrentUserName();

        /// <summary>
        /// Obtiene el rol del usuario del token JWT
        /// </summary>
        /// <returns>El rol del usuario o null si no está disponible</returns>
        string? GetCurrentUserRole();
    }
}
