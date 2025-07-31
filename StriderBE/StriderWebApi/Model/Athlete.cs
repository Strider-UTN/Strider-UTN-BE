namespace StriderWebApi.Model;

public class Athlete(int id, string username, string name, string password, string email, Gender gender, string address, double vO2Max, List<string> medicalConditions, List<GarminWorkout> workouts) : User(id, username, name, password, email, gender, address)
{
    public Team? Team { get; set; } = null;

    public double VO2Max { get; set; } = vO2Max;

    public List<string> MedicalConditions { get; } = medicalConditions;

    public List<InprogressInjury> InprogressInjuries { get; } = [];

    public List<RecoveredInjury> RecoveredInjuries { get; } = [];

    public List<GarminWorkout> Workouts { get; } = workouts;

    public void AddWorkouts(List<GarminWorkout> workouts) => Workouts.AddRange(workouts);

    public void UpdateVO2Max(double vO2Max) => VO2Max = vO2Max;

    public double Speed(int percentage) => VO2Max * percentage / 100;

    public void AddMedicalCondition(string medicalCondition) => MedicalConditions.Add(medicalCondition);

    public double WeeklyDistance()
    {
        DateTime today = DateTime.Now;
        return Workouts.Where(w => w.Date > today.AddDays(-7)).Sum(w => w.Distance);
    }

}

