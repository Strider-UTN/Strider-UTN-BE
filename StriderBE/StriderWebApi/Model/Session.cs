namespace StriderWebApi.Model;


public class Session
{

    private int _year { get; }
    private int _week { get; }
    private DayOfWeek _dayOfWeek { get; }

    private string _label { get; set; }

    private string _comments { get; }

    private Dictionary<Athlete, List<Lap>> _laps { get; }

    public int Year => _year;

    public int Week => _week;

    public void Label(string label) => _label = label;

    public Session(int year, int week, DayOfWeek dayOfWeek, string label, string comments, Dictionary<Athlete, List<Lap>> laps)
    {
        _year = year;
        _week = week;
        _dayOfWeek = dayOfWeek;
        _label = label;
        _comments = comments;
        _laps = laps;
    }

    public double TotalDistanceFor(Athlete athlete)
    {
        return _laps[athlete] == null ? 0 : _laps[athlete].Sum(l => l.Distance);
    }

    public double TotalTimeFor(Athlete athlete)
    {
        return _laps[athlete] == null ? 0 : _laps[athlete].Sum(l => l.Duration);
    }

}