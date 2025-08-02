namespace StriderWebApi.Model;

public class Team(string name, List<TrainingLocation> trainingLocations, DateTime creationDate)
{
    private readonly string _name = name;
    private readonly DateTime _creationDate = creationDate;
    private readonly List<TrainingLocation> _trainingLocations = trainingLocations;
    private bool _isPublic = false;
    private bool _automaticInscription = false;
    private bool _requiresManualApproval = false;
    private bool _isActive = true;
    private readonly List<Athlete> _athletes = [];
    private readonly List<Coach> _coaches = [];

    public string Name => _name;

    public DateTime CreationDate => _creationDate;

    public List<TrainingLocation> TrainingLocations => _trainingLocations;

    public bool IsPublic 
    { 
        get => _isPublic; 
        set => _isPublic = value; 
    }

    public bool AutomaticInscription 
    { 
        get => _automaticInscription; 
        set => _automaticInscription = value; 
    }

    public bool RequiresManualApproval 
    { 
        get => _requiresManualApproval; 
        set => _requiresManualApproval = value; 
    }

    public bool IsActive 
    { 
        get => _isActive; 
        set => _isActive = value; 
    }

    public List<Athlete> Athletes => _athletes;

    public List<Coach> Coaches => _coaches;

    public void AddAthlete(Athlete athlete) => _athletes.Add(athlete);

    public void AddCoach(Coach coach) => _coaches.Add(coach);

    public bool CreatedLastMonth(DateTime today)
    {
        return _creationDate.Month == today.Month && _creationDate.Year == today.Year;
    }

    public int TotalAthletes()
    {
        return _athletes.Count;
    }

    public int TotalTrainingLocations()
    {
        return _trainingLocations.Count;
    }
}