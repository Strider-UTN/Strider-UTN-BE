namespace StriderWebApi.Model;

public interface IIntervalSpeed
{
    double GetSpeed(Athlete athlete, double? speed, int? percentage);
}

public class FixedSpeed : IIntervalSpeed
{
    public double GetSpeed(Athlete athlete, double? speed, int? percentage)
    {
        return speed ?? throw new ArgumentNullException(nameof(speed));
    }
}

public class PercentageSpeed : IIntervalSpeed
{
    public double GetSpeed(Athlete athlete, double? speed, int? percentage)
    {
        return athlete.Speed(percentage ?? throw new ArgumentNullException(nameof(percentage)));
    }
} 