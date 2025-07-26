namespace StriderWebApi.Model;

public interface ISpeed
{
    public double Speed(Athlete athlete);

}

public class FixedSpeed(double speed) : ISpeed
{
    public double Speed(Athlete athlete) => speed;
}

public class VO2MaxSpeed(int percentage) : ISpeed
{
    public double Speed(Athlete athlete) => athlete.Speed(percentage);
}
