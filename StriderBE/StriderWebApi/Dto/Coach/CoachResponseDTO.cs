namespace StriderWebApi.Dto.Coach
{
    public class CoachResponseDTO(string name, int totalAthletes, int activeAthletes, int inactiveAthletes, int workoutsCompleted, List<CoachResponseAthleteDTO> coachResponseAthleteDTOs)
    {
        private List<CoachResponseAthleteDTO> coachResponseAthleteDTOs = coachResponseAthleteDTOs;

        public string Name { get; } = name;
        public int TotalAthletes { get; } = totalAthletes;
        public int ActiveAthletes { get; } = activeAthletes;
        public int InactiveAthletes { get; } = inactiveAthletes;
        public int WorkoutsCompleted { get; } = workoutsCompleted;
    }

    public class CoachResponseAthleteDTO(int id, string name, int age, List<string> objectives, int totalWorkouts, DateTime lastWorkoutDate, bool isActive, List<CoachResponseAthleteAilmentDTO> ailments)
    {
        public int Id { get; } = id;
        public string Name { get; } = name;
        public int Age { get; } = age;
        public List<string> Objectives { get; } = objectives;
        public int TotalWorkouts { get; } = totalWorkouts;
        public DateTime LastWorkoutDate { get; } = lastWorkoutDate;
        public bool IsActive { get; } = isActive;
        public List<CoachResponseAthleteAilmentDTO> Ailments { get; } = ailments;
    }

    public class CoachResponseAthleteAilmentDTO(string name, string treatment)
    {
        public string Name { get; } = name;
        public string Treatment { get; } = treatment;
    }
}