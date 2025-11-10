using System.ComponentModel.DataAnnotations;

namespace StriderWebApi.Dto.Invitation
{
    /// <summary>
    /// DTO para invitar a un atleta a una sede
    /// </summary>
    public class InviteGroupMemberDto
    {
        [Required(ErrorMessage = "El ID del atleta es requerido")]
        public int AthleteId { get; set; }

        [MaxLength(500, ErrorMessage = "El mensaje no puede exceder 500 caracteres")]
        public string? Message { get; set; }
    }
}
