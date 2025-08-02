namespace StriderWebApi.Model;

public class TrainingPlan(string name)
{
    private string _name = name;
    private readonly List<Coach> _coaches = [];
    private readonly List<Session> _sessions = [];
    private readonly List<Athlete> _athletes = [];

    public string Name 
    { 
        get => _name; 
        set => _name = value; 
    }
    
    public List<Coach> Coaches => _coaches;
    public List<Session> Sessions => _sessions;
    public List<Athlete> Athletes => _athletes;

    public void AddSession(Session session) => _sessions.Add(session);

    public void AddAthlete(Athlete athlete) => _athletes.Add(athlete);

    public void RemoveSession(Session session) => _sessions.Remove(session);

    public void RemoveAthlete(Athlete athlete) => _athletes.Remove(athlete);
}