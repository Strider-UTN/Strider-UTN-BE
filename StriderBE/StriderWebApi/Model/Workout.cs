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

public class Workout(int id, string name, int distance, DateTime date, double duration, List<Lap> laps)
{
    public int Id { get; } = id;
    public string Name { get; } = name;

    public int Distance { get;  } = distance;

    public DateTime Date { get;  } = date;

    public double Duration { get;  } = duration;

    public List<Lap> Laps { get;  } = laps;

    public WorkoutState State { get; set; } = WorkoutState.COMPLETED;

    public WorkoutType Type { get; set; } = WorkoutType.TRAINING;

}
