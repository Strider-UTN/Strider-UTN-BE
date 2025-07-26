namespace StriderWebApi.Model;

public class Team(string name, List<TrainingLocation> trainingLocations, DateTime creationDate)
{

    public string Name { get; } = name;

    public DateTime CreationDate { get; } = creationDate;

    public List<TrainingLocation> TrainingLocations { get; } = trainingLocations;

    public bool IsPublic { get; set; } = false;

    public bool AutomaticInscription { get; set; } = false;

    public bool RequiresManualApproval { get; set; } = false;

    public List<Athlete> Athletes { get; } = new();

    public List<Coach> Coaches { get; } = new();

    public void AddAthlete(Athlete athlete) => Athletes.Add(athlete);

    public void AddCoach(Coach coach) => Coaches.Add(coach);

    public Athlete? GetAthlete(User user) => Athletes.Find(a => a.User == user);
}