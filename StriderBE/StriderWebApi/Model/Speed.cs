namespace StriderWebApi.Model;

public interface ISpeed
{
    double Speed(Athlete athlete);
}

class FixedSpeed(double speed) : ISpeed
{

    private double _speed = speed;

    public double Speed(Athlete athlete) => _speed;
}

class PercentageSpeed(int percentage) : ISpeed
{

    private int _percentage = percentage;
    public double Speed(Athlete athlete) => athlete.Speed(_percentage);
}

