using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Model;

public class Athlete : User
{
    public double HeightCm { get; set; }
    public double WeightKg { get; set; }
    public string Country { get; set; } = string.Empty;
    public double? VO2Max { get; set; }
    public List<string> MedicalConditions { get; set; } = [];
    public List<string> Objectives { get; set; } = [];
    public List<Ailment> Ailments { get; set; } = [];
    public List<Workout> Workouts { get; set; } = [];
    public void AddWorkouts(List<Workout> workouts) => Workouts.AddRange(workouts);
    public void UpdateVO2Max(double vO2Max) => VO2Max = vO2Max;
    public double Speed(int percentage) => (VO2Max ?? 0) * percentage / 100;
    public void AddMedicalCondition(string medicalCondition) => MedicalConditions.Add(medicalCondition);
    
    public double TotalDistance(DateTime start, DateTime end)
    {
        return Workouts.Where(w => w.Date >= start && w.Date <= end).Sum(w => w.Distance);
    }
    
    public bool IsActive() => Ailments.Any(a => !a.IsRecovered);
    public bool IsInactive() => !IsActive();
    
    public DateTime GetLastWorkoutDate()
    {
        return Workouts.OrderBy(w => w.Date).Last().Date;
    }
    
    public int TotalWorkoutsCompleted() => Workouts.Count;
    public List<Ailment> GetActiveAilments() => [.. Ailments.Where(a => !a.IsRecovered)];
    public List<Workout> WorkoutsPendingFeedback() => [.. Workouts.Where(w => w.HasLinkedSession() && !w.HasFeedback())];
    public List<Workout> WorkoutsWithFeedback() => [.. Workouts.Where(w => w.HasLinkedSession() && w.HasFeedback())];
    public List<Workout> WorkoutsThisWeek() => [.. Workouts.Where(w => w.Date > DateTime.Now.AddDays(-7))];
    public Workout GetWorkoutById(int workoutId) => Workouts.First(w => w.Id == workoutId);
}

