namespace StriderWebApi.Model;

public class Coach(int id, string username, string name, string email, Gender gender, string address, DateTime birthDate) : User(id, username, name, email, gender, address, birthDate)
{
    private readonly List<Team> _teams = [];
    private readonly List<TrainingPlan> _trainingPlans = [];
    private readonly List<Session> _templates = [];
    private readonly List<Athlete> _athletes = [];

    public List<Team> Teams => _teams;

    public List<TrainingPlan> TrainingPlans => _trainingPlans;

    public List<Session> Templates => _templates;

    public List<Athlete> Athletes => _athletes;

    public void AddTeam(Team team) => _teams.Add(team);

    public void RemoveTeam(Team team) => _teams.Remove(team);

    public void AddTrainingPlan(TrainingPlan trainingPlan) => _trainingPlans.Add(trainingPlan);

    public void RemoveTrainingPlan(TrainingPlan trainingPlan) => _trainingPlans.Remove(trainingPlan);

    public void AddTemplate(Session template) => _templates.Add(template);

    public void AddAthlete(Athlete athlete) => _athletes.Add(athlete);

    public int TotalIndividualAthletes() => _athletes.Count;

    public int ActiveIndividualAthletes() => _athletes.FindAll(a => a.IsActive()).Count;

    public int InactiveIndividualAthletes() => _athletes.FindAll(a => a.IsInactive()).Count;

    public int ActiveTeams() => _teams.FindAll(t => t.IsActive).Count;

    public int NewTeamsSinceLastMonth() => _teams.FindAll(t => t.CreatedLastMonth(DateTime.Today)).Count;

    public int AthletesInTeams() => _teams.Sum(t => t.TotalAthletes());

    public int TotalTrainingLocations() => _teams.Sum(t => t.TotalTrainingLocations());

    public int TotalWorkoutsCompletedByIndividualAthletes() => _athletes.Sum(a => a.TotalWorkoutsCompleted());
}