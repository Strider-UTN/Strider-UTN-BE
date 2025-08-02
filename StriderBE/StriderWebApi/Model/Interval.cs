namespace StriderWebApi.Model;

public interface IInterval
{
    double Distance(Athlete athlete);
    double Duration(Athlete athlete);
    bool IsActive();

    ISpeed Speed { get; }
}

public class FixedDistance(double distance, ISpeed speed) : IInterval
{
    private readonly double _distance = distance;
    private readonly ISpeed _speed = speed;

    public double Distance(Athlete a) => _distance;
    public double Duration(Athlete a) => _distance / _speed.Speed(a);
    public bool IsActive() => true;
    public ISpeed Speed => _speed;
}

public class FixedDuration(double duration, ISpeed speed) : IInterval
{
    private readonly double _duration = duration;
    private readonly ISpeed _speed = speed;

    public double Distance(Athlete athlete) => _speed.Speed(athlete) * _duration;
    public double Duration(Athlete athlete) => _duration;
    public bool IsActive() => true;
    public ISpeed Speed => _speed;
}

public class Rest(double duration) : IInterval
{
    private readonly double _duration = duration;

    public double Distance(Athlete athlete) => 0;
    public double Duration(Athlete athlete) => _duration;
    public bool IsActive() => false;
    public ISpeed Speed => new FixedSpeed(0);
}

