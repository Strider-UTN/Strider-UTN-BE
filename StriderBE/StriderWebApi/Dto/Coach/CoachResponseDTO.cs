namespace StriderWebApi.Dto.Coach
{
    public class CoachResponseDTO
    {
        public string Name { get; set; } = string.Empty;
        public int TotalAthletes { get; set; }
        public int ActiveAthletes { get; set; }
        public int InactiveAthletes { get; set; }
        public int WorkoutsCompleted { get; set; }
        public List<Athlete> Athletes { get; set; } = [];

        public class Athlete
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public int Age { get; set; }
            public List<string> Objectives { get; set; } = [];
            public int TotalWorkouts { get; set; }
            public DateTime LastWorkoutDate { get; set; }
            public bool IsActive { get; set; }
            public List<Ailment> Ailments { get; set; } = [];

            public class Ailment
            {
                public string Name { get; set; } = string.Empty;
                public string Treatment { get; set; } = string.Empty;
            }
        }
    }
}