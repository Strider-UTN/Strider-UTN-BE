using StriderWebApi.Model;

public class APIWorkout
{
    public string Name { get; set; } = string.Empty;
    public double Distance { get; set; }
    public DateTime Date { get; set; }
    public double Duration { get; set; }
    public double AverageHR { get; set; }
    public List<APILap> Laps { get; set; } = [];

    public Workout ToWorkout(Athlete athlete)
    {
        return new Workout(Name, Distance, Date, Duration, AverageHR, [.. Laps.Select(l => new Lap(l.Index, l.Distance, l.Duration,l.Speed,l.AverageHR, l.StartTime))], string.Empty, athlete);
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