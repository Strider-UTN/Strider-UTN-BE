using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Model;

public class Session
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Warmup { get; set; } = string.Empty;
    public string CoolDown { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? Comments { get; set; }
    public WorkoutType SessionType { get; set; }    
    public List<Interval> Intervals { get; set; } = [];
    public List<Workout> Workouts { get; set; } = [];

    public double TotalDistance(Athlete athlete)
    {
        return Intervals.Sum(l => l.GetDistance(athlete));
    }

    public double TotalDuration(Athlete athlete)
    {
        return Intervals.Sum(l => l.GetDuration(athlete));
    }
    
    public int IntervalCount() => Intervals.Count;
    public int ActiveIntervalCount() => Intervals.FindAll(i => i.IsActive).Count;
    public Session Clone() => new() 
    { 
        Date = Date, 
        Warmup = Warmup, 
        CoolDown = CoolDown, 
        Description = Description, 
        Name = Name, 
        Category = Category, 
        Label = Label, 
        Comments = Comments, 
        Intervals = Intervals, 
        SessionType = SessionType 
    };
    public Interval GetInterval(int index) => Intervals[index];
}
