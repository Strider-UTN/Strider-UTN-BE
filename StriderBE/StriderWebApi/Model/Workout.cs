namespace StriderWebApi.Model;


public enum WorkoutState
{
    COMPLETED,
    ABANDONED,

}

public enum WorkoutType
{
    TRAINING,
    COMPETITION
}

public class WorkoutLap(int Index, double distance, double duration, double speed, DateTime startTime)
{
    public int Index { get; } = Index;
    
    public double Distance { get; } = distance;
    public double Duration { get; } = duration;
    public double Speed { get; } = speed;

    public DateTime StartTime { get; } = startTime;
}

public class Workout(int id, string name, int distance, DateTime date, double duration, List<WorkoutLap> laps)
{

    public int Id { get; } = id;
    public string Name { get; } = name;

    public int Distance { get; } = distance;

    public DateTime Date { get; } = date;

    public double Duration { get; } = duration;

    public List<WorkoutLap> Laps { get; } = laps;

    public WorkoutState State { get; set; } = WorkoutState.COMPLETED;

    public WorkoutType Type { get; set; } = WorkoutType.TRAINING;

}
