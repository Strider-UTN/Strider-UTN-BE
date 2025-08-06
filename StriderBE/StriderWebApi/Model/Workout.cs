using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Model;

public class Lap
{
    public int Id { get; set; }
    public int Index { get; set; }
    public double Distance { get; set; }
    public double Duration { get; set; }
    public double Speed { get; set; }
    public double HR { get; set; }
    public DateTime StartTime { get; set; }
    public string? CoachFeedback { get; set; }
    
}

public class Workout
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Distance { get; set; }
    public DateTime Date { get; set; }
    public double Duration { get; set; }
    public double AverageHR { get; set; }
    public WorkoutState State { get; set; }
    public WorkoutType Type { get; set; }
    public string? Comments { get; set; }
    public string? CoachFeedback { get; set; }
    public bool IsReviewed { get; set; }
    
    public Athlete Athlete { get; set; } = null!;
    public Session? Session { get; set; }
    public List<Lap> Laps { get; set; } = [];

    public bool HasLinkedSession() => Session != null;
    public double TotalDistance() => Laps.Sum(l => l.Distance);
    public double AverageSpeed() => Laps.Average(l => l.Speed);
    public Lap GetLap(int index) => Laps[index];
    public bool HasFeedback() => !string.IsNullOrEmpty(CoachFeedback);
}

public class WorkoutComparer
{
    readonly double _durationWeight = 0.2;
    readonly double _distanceWeight = 0.2;
    readonly double _speedWeight = 0.6;
    readonly double _reductionParameter = 0.001;

    public int MatchPercentage(Athlete athlete, Lap lap, Interval interval)
    {
        double durationDiff = Math.Abs(lap.Duration - interval.GetDuration(athlete));
        double distanceDiff = Math.Abs(lap.Distance - interval.GetDistance(athlete));
        double speedDiff = Math.Abs(lap.Speed - interval.SpeedType.GetSpeed(athlete, interval.Speed, interval.Percentage));
        double totalDiff = durationDiff * _durationWeight + distanceDiff * _distanceWeight + speedDiff * _speedWeight;
        return (int)(Math.Exp(-1 * totalDiff * _reductionParameter) * 100);
    }

    public int AverageCompletionPercentage(Athlete athlete, Workout workout, Session session)
    {
        int total = 0;
        foreach (Lap lap in workout.Laps)
        {
            total += MatchPercentage(athlete, lap, session.GetInterval(lap.Index));
        }
        return total / workout.Laps.Count;
    }
}
