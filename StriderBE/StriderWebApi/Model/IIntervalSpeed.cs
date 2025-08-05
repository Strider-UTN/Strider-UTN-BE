namespace StriderWebApi.Model;

public interface IIntervalSpeed
{
    double GetSpeed(Athlete athlete, double speed, int percentage);
}

public class FixedSpeed : IIntervalSpeed
{
    public double GetSpeed(Athlete athlete, double speed, int percentage)
    {
        return speed;
    }
}

public class PercentageSpeed : IIntervalSpeed
{
    public double GetSpeed(Athlete athlete, double speed, int percentage)
    {
        return athlete.Speed(percentage);
    }
} 