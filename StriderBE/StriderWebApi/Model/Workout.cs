namespace StriderWebApi.Model;


public enum WorkoutState
{
    COMPLETED,
    ABANDONED,

}

public class Lap(int index, double distance, double duration, double speed,double hr, DateTime startTime)
{
    private readonly int _index = index;
    private readonly double _distance = distance;
    private readonly double _duration = duration;
    private readonly double _speed = speed;
    private readonly DateTime _startTime = startTime;
    private readonly double _hr = hr;
    private readonly string _coachFeedback = "";
    public int Index => _index;
    public double Distance => _distance;
    public double Duration => _duration;
    public double Speed => _speed;
    public double HR => _hr;
    public string CoachFeedback => _coachFeedback;
    public DateTime StartTime => _startTime; 
}
public class Workout(string name, double distance, DateTime date, double duration, double averageBPM, List<Lap> laps, string comments, Athlete athlete, Session? session = null)
{
    private readonly string _name = name;
    private readonly double _distance = distance;
    private readonly DateTime _date = date;
    private readonly double _duration = duration;
    private double _averageBPM = averageBPM;
    private readonly List<Lap> _laps = laps;
    private WorkoutState _state = WorkoutState.COMPLETED;
    private WorkoutType _type = WorkoutType.TRAINING;
    private string _comments = comments;
    private string _coachFeedback = "";
    private bool _isReviewed = false;
    private readonly Session? _linkedSession = session;
    private readonly Athlete _athlete = athlete;
    public string Name => _name;
    public double Distance => _distance;
    public DateTime Date => _date;
    public double Duration => _duration;

    public double AverageBPM
    {
        get => _averageBPM;
        set => _averageBPM = value;
    }

    public List<Lap> Laps => _laps;

    public WorkoutState State
    {
        get => _state;
        set => _state = value;
    }

    public WorkoutType Type
    {
        get => _type;
        set => _type = value;
    }

    public string Comments
    {
        get => _comments;
        set => _comments = value;
    }

    public string CoachFeedback
    {
        get => _coachFeedback;
        set => _coachFeedback = value;
    }

    public bool IsReviewed
    {
        get => _isReviewed;
        set => _isReviewed = value;
    }

    public Session? LinkedSession => _linkedSession;

    public Athlete Athlete => _athlete;

    public bool HasLinkedSession() => _linkedSession != null;
    public double TotalDistance() => _laps.Sum(l => l.Distance);
    public double AverageSpeed() => _laps.Average(l => l.Speed);
    internal Lap GetLap(int index) => _laps[index];
}

public class LapIntervalComparer(Athlete athlete, Lap lap, IInterval interval)
{

    readonly double _durationWeight = 0.2;
    readonly double _distanceWeight = 0.2;
    readonly double _speedWeight = 0.6;
    readonly double _reductionParameter = 0.1;

    public int MatchPercentage()
    {
        double durationDiff = Math.Abs(lap.Duration - interval.Duration(athlete));
        double distanceDiff = Math.Abs(lap.Distance - interval.Distance(athlete));
        double speedDiff = Math.Abs(lap.Speed - interval.Speed.Speed(athlete));
        double totalDiff = durationDiff * _durationWeight + distanceDiff * _distanceWeight + speedDiff * _speedWeight;
        return (int)(Math.Exp(-1 * totalDiff * _reductionParameter) * 100);
    }
}
