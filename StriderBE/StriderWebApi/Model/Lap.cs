namespace StriderWebApi.Model;


public class Lap(double distance, double duration)
{
    private double _distance { get; set; } = distance;
    private double _duration { get; set; } = duration;

    public double Speed => _distance / _duration;

    public double Distance => _distance;

    public double Duration => _duration;

}

