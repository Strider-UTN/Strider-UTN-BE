using System.ComponentModel.DataAnnotations;

namespace StriderWebApi.Dto.Invitation
{
    /// <summary>
    /// DTO para responder a una invitación de sede
    /// </summary>
    public class RespondToGroupInvitationDto
    {
        [Required(ErrorMessage = "La decisión es requerida")]
        public bool Accept { get; set; }
    }
}
