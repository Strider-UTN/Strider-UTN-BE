using System.ComponentModel.DataAnnotations;

namespace StriderWebApi.Dto.UserCreation
{
    public class UpdateThemeDto
    {
        [Required]
        [RegularExpression("^(light|dark)$", ErrorMessage = "El tema debe ser 'light' o 'dark'")]
        public string Theme { get; set; } = string.Empty;
    }
}
