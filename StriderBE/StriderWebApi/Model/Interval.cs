namespace StriderWebApi.Model;

public interface IInterval
{
    double Distance(Athlete athlete);
    double Duration(Athlete athlete);
    ISpeed Speed { get; }
}

public class FixedDistance(double distance, ISpeed speed) : IInterval
{

    public double distance = distance;
    public ISpeed speed = speed;

    public double Distance(Athlete a) => distance;
    public double Duration(Athlete a) => distance / speed.Speed(a);
    public ISpeed Speed => speed;

}

public class FixedDuration(double duration, ISpeed speed) : IInterval
{
    public double Distance(Athlete athlete) => speed.Speed(athlete) * duration;
    public double Duration(Athlete athlete) => duration;
    public ISpeed Speed => speed;
}

public class Rest(double duration) : IInterval
{

    public double duration = duration;

    public double Distance(Athlete athlete) => 0;
    public double Duration(Athlete athlete) => duration;
    public ISpeed Speed => new FixedSpeed(0);
}

