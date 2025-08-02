namespace StriderWebApi.Model;

public interface ISpeed
{
    double Speed(Athlete athlete);
}

public class FixedSpeed(double speed) : ISpeed
{
    private readonly double _speed = speed;

    public double Speed(Athlete athlete) => _speed;
}

public class PercentageSpeed(int percentage) : ISpeed
{
    private readonly int _percentage = percentage;

    public double Speed(Athlete athlete) => athlete.Speed(_percentage);
}

