namespace StriderWebApi.Dto.Athlete
{
    public class AthleteFeedbackResponseDTO
    {
        public int WorkoutsPendingFeedback { get; set; }
        public int WorkoutsWithFeedback { get; set; }
        public int WorkoutsThisWeek { get; set; }
        public List<Workout> Workouts { get; set; } = [];

        public class Workout
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public DateTime Date { get; set; }
            public string Comments { get; set; } = string.Empty;
            public double Duration { get; set; }
            public double AverageHR { get; set; }
            public int Count { get; set; }
            public int ActiveIntervalCount { get; set; }
            public int Value { get; set; }
            public List<Interval> Intervals { get; set; } = [];

            public class Interval
            {
                public int Index { get; set; }
                public bool IsActive { get; set; }
                public Planned PlannedInterval { get; set; } = null!;
                public Actual ActualInterval { get; set; } = null!;
                public int MatchPercentage { get; set; }

                public class Planned
                {
                    public double Distance { get; set; }
                    public double Duration { get; set; }
                    public double Speed { get; set; }
                }

                public class Actual
                {
                    public double Distance { get; set; }
                    public double Duration { get; set; }
                    public double Speed { get; set; }
                    public double HR { get; set; }
                }
            }
        }
    }
}
