namespace StriderWebApi.Model;

public class Lap(int index, DateTime startTime, double distance, double duration, double averageSpeed)
{
    public int Index { get; set; } = index;

    public DateTime StartTime { get; set; } = startTime;
    public double Distance { get; set; } = distance;
    public double Duration { get; set; } = duration;
    public double AverageSpeed { get; set; } = averageSpeed;
}