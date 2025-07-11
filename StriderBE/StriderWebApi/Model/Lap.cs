namespace StriderWebApi.Model;

public class Lap
{
    public int Index { get; set; }

    public DateTime StartTime { get; set; }
    public double Distance { get; set; }
    public TimeSpan Duration { get; set; }
    public double AverageSpeed { get; set; }

    public Lap(int index, DateTime startTime, double distance, TimeSpan duration, double averageSpeed)
    {
        StartTime = startTime;
        Index = index;
        Distance = distance;
        Duration = duration;
        AverageSpeed = averageSpeed;
    }
}