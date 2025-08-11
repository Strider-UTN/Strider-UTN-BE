using StriderWebApi.Model;

namespace StriderWebApi.Model;

public class Interval
{
    public int Id { get; set; }
    public int Repetitions { get; set; }
    public double Rest { get; set; }
    public double? Distance { get; set; }
    public double? Duration { get; set; }
    public double? Speed { get; set; }
    public int? Percentage { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public IIntervalType Type { get; set; } = null!;
    public IIntervalSpeed SpeedType { get; set; } = null!;

    public double GetDistance(Athlete athlete) => Type.GetDistance(Distance, Duration, SpeedType.GetSpeed(athlete, Speed, Percentage));
    public double GetDuration(Athlete athlete) => Type.GetDuration(Distance, Duration, SpeedType.GetSpeed(athlete, Speed, Percentage));
    public double GetSpeed(Athlete athlete) => SpeedType.GetSpeed(athlete, Speed, Percentage);

    public Interval Clone() => new()
    {
        Distance = Distance,
        Duration = Duration,
        Speed = Speed,
        Percentage = Percentage,
        IsActive = IsActive,
        Description = Description,
        Type = Type,
        SpeedType = SpeedType,
        Repetitions = Repetitions
    };
}

