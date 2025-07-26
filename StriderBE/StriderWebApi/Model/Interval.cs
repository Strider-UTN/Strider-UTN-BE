namespace StriderWebApi.Model;

public interface IInterval
{
    public double Distance { get; }
    public double Duration(Athlete athlete);
    public double Speed(Athlete athlete) => Distance / Duration(athlete);
}

public class Run(double distance, ISpeed pace) : IInterval
{
    public double Distance = distance;

    public ISpeed Pace = pace;

    double IInterval.Distance => Distance;

    public double Duration(Athlete athlete) => Distance / Pace.Speed(athlete);
}

public class Rest(double duration) : IInterval
{
    public double Duration = duration;

    public double Distance => throw new NotImplementedException();

    double IInterval.Duration(Athlete athlete)
    {
        return Duration;
    }
}

