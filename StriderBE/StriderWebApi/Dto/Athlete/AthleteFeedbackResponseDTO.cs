namespace StriderWebApi.Dto.Athlete
{
    public class AthleteFeedbackResponseDTO(int workoutsPendingFeedback, int workoutsWithFeedback, int workoutsThisWeek, List<AthleteFeedbackResponseDTO.Workout> workouts)
    {

        public int WorkoutsPendingFeedback = workoutsPendingFeedback;
        readonly int WorkoutsWithFeedback = workoutsWithFeedback;
        readonly int WorkoutsThisWeek = workoutsThisWeek;
        readonly List<Workout> workouts = workouts;

        public class Workout(int id, string name, DateTime date, double duration, double averageHR, string comments, int count, int activeIntervalCount, int value, List<Workout.Interval> intervals)
        {
            public int Id { get; } = id;
            public string Name { get; } = name;
            public DateTime Date { get; } = date;
            public string Comments { get; } = comments;
            public double Duration { get; } = duration;
            public double AverageHR { get; } = averageHR;
            public int Count { get; } = count;
            public int ActiveIntervalCount { get; } = activeIntervalCount;
            public int Value { get; } = value;
            public List<Interval> Intervals { get; } = intervals;

            public class Interval(int index, bool isActive, Interval.Planned plannedInterval, Interval.Actual actualInterval, int matchPercentage)
            {
                public int Index { get; } = index;
                public bool IsActive { get; } = isActive;
                public Planned PlannedInterval { get; } = plannedInterval;
                public Actual ActualInterval { get; } = actualInterval;
                public int MatchPercentage { get; } = matchPercentage;

                public class Planned(double distance, double duration, double speed)
                {
                    public double Distance { get; } = distance;
                    public double Duration { get; } = duration;
                    public double Speed { get; } = speed;
                }

                public class Actual(double distance, double duration, double speed, double hR)
                {
                    public double Distance { get; } = distance;
                    public double Duration { get; } = duration;
                    public double Speed { get; } = speed;
                    public double HR { get; } = hR;
                }
            }


        }

    }
}
