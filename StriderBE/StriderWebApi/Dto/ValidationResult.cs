namespace StriderWebApi.Dto
{
    /// <summary>
    /// Clase auxiliar para resultados de validación
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
