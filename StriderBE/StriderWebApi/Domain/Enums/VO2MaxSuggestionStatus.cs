namespace StriderWebApi.Domain.Enums
{
    /// <summary>
    /// Estado de una sugerencia de VO2Max
    /// </summary>
    public enum VO2MaxSuggestionStatus
    {
        /// <summary>
        /// Pendiente de respuesta del atleta
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Aceptada por el atleta
        /// </summary>
        Accepted = 1,

        /// <summary>
        /// Rechazada por el atleta
        /// </summary>
        Rejected = 2
    }
}

