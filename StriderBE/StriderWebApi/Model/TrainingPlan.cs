namespace StriderWebApi.Model;

public class TrainingPlan(string name)
{
    public string Name { get; set; } = name;
    public List<Coach> Coaches { get; } = [];
    public List<Session> Sessions { get; } = [];
    public List<Athlete> Athletes { get; } = [];

    public void AddSession(Session session) => Sessions.Add(session);

    public void AddAthlete(Athlete athlete) => Athletes.Add(athlete);

    public void RemoveSession(Session session) => Sessions.Remove(session);

    public void RemoveAthlete(Athlete athlete) => Athletes.Remove(athlete);

}