using StriderWebApi.Model;

namespace StriderWebApi.Model;

public class Interval
{
    public int Id { get; set; }
    public double Distance { get; set; }
    public double Duration { get; set; }
    public double Speed { get; set; }
    public int Percentage { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public IIntervalType Type { get; set; } = null!;
    public IIntervalSpeed SpeedType { get; set; } = null!;

    public double GetDistance(Athlete athlete)
    {
        return Type.GetDistance(Distance, Duration, SpeedType.GetSpeed(athlete, Speed, Percentage));
    }

    public double GetDuration(Athlete athlete)
    {
        return Type.GetDuration(Distance, Duration, SpeedType.GetSpeed(athlete, Speed, Percentage));
    }
}



