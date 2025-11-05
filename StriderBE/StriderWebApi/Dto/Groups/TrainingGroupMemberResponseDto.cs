using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Dto.Groups
{
    /// <summary>
    /// DTO de respuesta para un miembro de una sede
    /// </summary>
    public class TrainingGroupMemberResponseDto
    {
        public int Id { get; set; }

        public int TrainingGroupId { get; set; }

        public string TrainingGroupName { get; set; } = string.Empty;

        public int UserId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? ProfileImage { get; set; }

        public string JoinedDate { get; set; } = string.Empty; // ISO format

        public TrainingGroupMemberStatus Status { get; set; }
    }
}
