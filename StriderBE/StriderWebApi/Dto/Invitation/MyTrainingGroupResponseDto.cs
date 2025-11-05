namespace StriderWebApi.Dto.Invitation
{
    /// <summary>
    /// DTO de respuesta para las sedes del atleta
    /// </summary>
    public class MyTrainingGroupResponseDto
    {
        public int Id { get; set; }

        public int TrainingGroupId { get; set; }

        public string TrainingGroupName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int CoachId { get; set; }

        public string CoachName { get; set; } = string.Empty;

        public string CoachEmail { get; set; } = string.Empty;

        public List<string> TrainingPoints { get; set; } = new();

        public int MemberCount { get; set; }

        public string JoinedDate { get; set; } = string.Empty; // ISO format
    }
}
