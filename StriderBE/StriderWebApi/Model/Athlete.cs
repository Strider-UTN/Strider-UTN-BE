namespace StriderWebApi.Model;

public class Athlete(User user, double vO2Max, List<string> medicalConditions, List<Workout> workouts)
{
    private User _user { get; } = user;

    private Team? _team { get; set; } = null;

    private double _vO2Max { get; set; } = vO2Max;

    private List<string> _medicalConditions { get; } = medicalConditions;

    private List<Workout> _workouts { get; } = workouts;

    public void AddWorkouts(List<Workout> workouts) => _workouts.AddRange(workouts);

    public void UpdateVO2Max(double vO2Max) => _vO2Max = vO2Max;

    public double Speed(int percentage) => _vO2Max * percentage / 100;

    public void AddTeam(Team team) => _team = team;

    public void AddMedicalCondition(string medicalCondition) => _medicalConditions.Add(medicalCondition);
    
}