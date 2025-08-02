namespace StriderWebApi.Model;

public class Athlete(int id, string username, string name, string password, string email, Gender gender, string address, double vO2Max, List<string> medicalConditions, List<string> objectives, DateTime birthdate) : User(id, username, name, password, email, gender, address, birthdate)
{
    private Team? _team;
    private double _vO2Max = vO2Max;
    private readonly List<string> _objectives = objectives;
    private readonly List<string> _medicalConditions = medicalConditions;
    private readonly List<Ailment> _ailments = [];
    private readonly List<Workout> _workouts = [];

    public Team? Team 
    { 
        get => _team; 
        set => _team = value; 
    }

    public double VO2Max 
    { 
        get => _vO2Max; 
        set => _vO2Max = value; 
    }

    public List<string> Objectives => _objectives;

    public List<string> MedicalConditions => _medicalConditions;

    public List<Ailment> Ailments => _ailments;

    public List<Workout> Workouts => _workouts;

    public void AddWorkouts(List<Workout> workouts) => _workouts.AddRange(workouts);

    public void UpdateVO2Max(double vO2Max) => _vO2Max = vO2Max;

    public double Speed(int percentage) => _vO2Max * percentage / 100;

    public void AddMedicalCondition(string medicalCondition) => _medicalConditions.Add(medicalCondition);

    public double TotalDistance(DateTime start, DateTime end)
    {
        DateTime today = DateTime.Now;
        return _workouts.Where(w => w.Date >= start && w.Date <= end).Sum(w => w.Distance);
    }

    internal bool IsActive()
    {
        return _ailments.Any(a => !a.IsRecovered());
    }

    internal bool IsInactive()
    {
        return !IsActive();
    }

    public DateTime GetLastWorkoutDate()
    {
        return _workouts.OrderBy(w => w.Date).Last().Date;
    }

    public int TotalWorkoutsCompleted() => _workouts.Count;

    public List<Ailment> GetActiveAilments() => [.. _ailments.Where(a => !a.IsRecovered())];
}

