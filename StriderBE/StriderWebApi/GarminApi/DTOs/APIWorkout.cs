using StriderWebApi.Model;
namespace StriderWebApi.GarminApi.DTOs;
public class APIWorkout
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Distance { get; set; }
    public DateTime Date { get; set; }
    public double Duration { get; set; }
    public double AverageHR { get; set; }
    public List<APILap> Laps { get; set; } = [];

    public Workout ToWorkout(Athlete athlete)
    {
        var workout = new Workout
        {
            Id = Id,
            Name = Name,
            Distance = Distance,
            Date = Date,
            Duration = Duration,
            AverageHR = AverageHR,
            Comments = string.Empty,
            Athlete = athlete,
        };
        
        workout.Laps = Laps.Select(l => new Lap
        {
            Index = l.Index,
            Distance = l.Distance,
            Duration = l.Duration,
            Speed = l.Speed,
            HR = l.AverageHR,
            StartTime = l.StartTime,
        }).ToList();
        
        return workout;
    }
}

public class APILap
{
    public int Index { get; set; }
    public double Distance { get; set; }
    public double Duration { get; set; }
    public double AverageHR { get; set; }
    public double Speed { get; set; }
    public DateTime StartTime { get; set; }
}