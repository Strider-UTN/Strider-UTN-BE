namespace StriderWebApi.Model;

public class Session(DateTime date, string warmup, string coolDown, string description, string name, string category, string label, string comments, List<Athlete> athletes, List<IInterval> intervals, WorkoutType sessionType = WorkoutType.TRAINING)
{
    private readonly DateTime _date = date;
    private readonly string _warmup = warmup;
    private readonly string _coolDown = coolDown;
    private readonly string _description = description;
    private readonly string _name = name;
    private readonly string _category = category;
    private readonly string _label = label;
    private readonly string _comments = comments;
    private readonly WorkoutType _sessionType = sessionType;
    private readonly List<Athlete> _athletes = athletes;
    private readonly List<IInterval> _intervals = intervals;

    public DateTime Date => _date;
    public string Warmup => _warmup;
    public string CoolDown => _coolDown;
    public string Description => _description;
    public string Name => _name;
    public string Category => _category;
    public string Label => _label;
    public string Comments => _comments;
    public WorkoutType SessionType => _sessionType;
    public List<Athlete> Athletes => _athletes;
    public List<IInterval> Intervals => _intervals;

    public double TotalDistance(Athlete athlete)
    {
        return _intervals.Sum(l => l.Distance(athlete));
    }

    public double TotalDuration(Athlete athlete)
    {
        return _intervals.Sum(l => l.Duration(athlete));
    }
    public int IntervalCount() => _intervals.Count;
    public int ActiveIntervalCount() => _intervals.FindAll(i => i.IsActive()).Count;
    public Session Clone() => new(_date, _warmup, _coolDown, _description, _name, _category, _label, _comments, [], _intervals, _sessionType);
    public IInterval GetInterval(int index) => _intervals[index];

}
