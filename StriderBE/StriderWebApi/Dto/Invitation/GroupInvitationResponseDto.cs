using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Dto.Invitation
{
    /// <summary>
    /// DTO de respuesta para una invitación a una sede
    /// </summary>
    public class GroupInvitationResponseDto
    {
        public int Id { get; set; }

        public int TrainingGroupId { get; set; }

        public string TrainingGroupName { get; set; } = string.Empty;

        public int CoachId { get; set; }

        public string CoachName { get; set; } = string.Empty;

        public string CoachEmail { get; set; } = string.Empty;

        public int AthleteId { get; set; }

        public string AthleteName { get; set; } = string.Empty;

        public string AthleteEmail { get; set; } = string.Empty;

        public TrainingGroupMemberStatus Status { get; set; }

        public string? InvitationMessage { get; set; }

        public string JoinedDate { get; set; } = string.Empty; // ISO format

        public string? RespondedAt { get; set; } // ISO format, null si no ha respondido
    }
}
