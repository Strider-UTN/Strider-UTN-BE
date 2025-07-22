namespace StriderWebApi.Model;

public class Team(string name)
{
    private string _name { get; } = name;

    private List<Calendar> _calendar { get; } = [];

    private List<Athlete> _athletes { get; } = [];

    private List<Coach> _coaches { get; } = [];

    public void AddAthlete(Athlete athlete) => _athletes.Add(athlete);

    public void AddCoach(Coach coach) => _coaches.Add(coach);

    public void AddCalendar(Calendar calendar) => _calendar.Add(calendar);

    public void RemoveCalendar(Calendar calendar) => _calendar.Remove(calendar);
 
}