using System.ComponentModel.DataAnnotations;

namespace StriderWebApi.Dto.Groups
{
    /// <summary>
    /// DTO para crear una nueva sede/grupo de entrenamiento
    /// </summary>
    public class CreateTrainingGroupDto
    {
        [Required(ErrorMessage = "El nombre de la sede es requerido")]
        [MaxLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe proporcionar al menos un punto de entrenamiento")]
        [MinLength(1, ErrorMessage = "Debe proporcionar al menos un punto de entrenamiento")]
        public List<string> TrainingPoints { get; set; } = new();

        [MaxLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        public string? Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El límite de miembros debe ser mayor a 0")]
        public int? MaxMembers { get; set; }

        public bool IsPublic { get; set; } = false;

        public bool AllowSelfJoin { get; set; } = false;

        public bool RequireApproval { get; set; } = true;
    }
}
