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
            public string Email { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public int Experience { get; set; }
            public double WeeklyDistance { get; set; }
            public int Age { get; set; }
            public int BirthYear { get; set; }
            public double Height { get; set; }
            public double Weight { get; set; }
            public double MonthlyDistance { get; set; }
            public string EmergencyContactName { get; set; } = string.Empty;
            public string EmergencyContactPhone { get; set; } = string.Empty;
            public string EmergencyContactRelationship { get; set; } = string.Empty;
            public DateTime RegistrationDate { get; set; }
            public DateTime LastActivityDate { get; set; }

        }
    }
}