namespace StriderWebApi.Model;

public interface IIntervalType
{
    double GetDistance(double? distance, double? duration, double speed);
    double GetDuration(double? distance, double? duration, double speed);
}

public class FixedDistance : IIntervalType
{
    public double GetDistance(double? distance, double? duration, double speed)
    {
        return distance ?? throw new ArgumentNullException(nameof(distance));
    }

    public double GetDuration(double? distance, double? duration, double speed)
    {
        return (distance ?? throw new ArgumentNullException(nameof(distance))) / speed;
    }
}

public class Rest : IIntervalType
{
    public double GetDistance(double? distance, double? duration, double speed)
    {
        return 0;
    }

    public double GetDuration(double? distance, double? duration, double speed)
    {
        return duration ?? throw new ArgumentNullException(nameof(duration));
    }
} 