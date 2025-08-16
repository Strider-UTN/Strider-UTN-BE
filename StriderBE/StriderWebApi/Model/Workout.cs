using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Model;

public class Lap
{
    public int Id { get; set; }
    public int Index { get; set; }
    public double Distance { get; set; }
    public double Duration { get; set; }
    public double Speed { get; set; }
    public int HR { get; set; }
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
    public List<Discomfort> Discomforts { get; set; } = [];
    public Athlete Athlete { get; set; } = null!;
    public Session? Session { get; set; }
    public List<Lap> Laps { get; set; } = [];
    public bool HasLinkedSession() => Session != null;
    public double TotalDistance() => Laps.Sum(l => l.Distance);
    public double AverageSpeed() => Laps.Average(l => l.Speed);
    public Lap GetLap(int index) => Laps[index];
    public bool HasFeedback() => !string.IsNullOrEmpty(CoachFeedback);
    public bool HasDiscomfort() => Discomforts.Count != 0;
}
