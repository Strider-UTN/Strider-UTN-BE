namespace StriderWebApi.Dto.Groups
{
    /// <summary>
    /// DTO para las estadísticas de una sede
    /// </summary>
    public class TrainingGroupStatsDto
    {
        public int TotalWorkouts { get; set; }

        public int CompletedWorkouts { get; set; }

        public int PlannedWorkouts { get; set; }

        public int ActiveMembers { get; set; }
    }
}
