namespace StriderWebApi.Model;


public class Session(DateTime date, string warmup, string coolDown, string description, string name, string category, string label, string comments, List<Athlete> athletes, List<IInterval> Intervals, WorkoutType sessionType = WorkoutType.TRAINING)
{
    public DateTime Date { get; } = date;
    public string Warmup { get; } = warmup;
    public string CoolDown { get; } = coolDown;
    public string Description { get; } = description;
    public string Name { get; } = name;
    public string Category { get; } = category;
    public string Label { get; } = label;
    public string Comments { get; } = comments;
    public WorkoutType SessionType { get; } = sessionType;
    public List<Athlete> Athletes { get; } = athletes;
    public List<IInterval> Intervals { get; } = Intervals;

    public double TotalDistance()
    {
        return Intervals.Sum(l => l.Distance);
    }

    public double TotalDuration(Athlete athlete)
    {
        return Intervals.Sum(l => l.Duration(athlete));
    }

    public Session Clone()
    {
        return new Session(Date, Warmup, CoolDown, Description, Name, Category, Label, Comments, [], Intervals, SessionType);
    }

}
