using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Model;

public class Athlete : User
{
    public double Height { get; set; }
    public double Weight { get; set; }
    public string Country { get; set; } = string.Empty;
    public string EmergencyContactName { get; set; } = string.Empty;
    public string EmergencyContactPhone { get; set; } = string.Empty;
    public string EmergencyContactRelationship { get; set; } = string.Empty;
    public DateTime DateStartedRunning { get; set; }
    public double VO2Max { get; set; }
    public int MinHeartRate { get; set; }
    public int MaxHeartRate { get; set; }
    public int ThresholdHeartRate { get; set; }
    public List<string> MedicalConditions { get; set; } = [];
    public List<string> Objectives { get; set; } = [];
    public List<Ailment> Ailments { get; set; } = [];
    public List<Workout> Workouts { get; set; } = [];
    public void AddWorkouts(List<Workout> workouts) => Workouts.AddRange(workouts);
    public void UpdateVO2Max(double vO2Max) => VO2Max = vO2Max;
    public double Speed(int percentage) => VO2Max * percentage / 100;
    public void AddMedicalCondition(string medicalCondition) => MedicalConditions.Add(medicalCondition);
    public double WeeklyDistance() => Workouts.Where(w => w.Date > DateTime.Now.AddDays(-7)).Sum(w => w.TotalDistance());
    public double MonthlyDistance() => Workouts.Where(w => w.Date > DateTime.Now.AddDays(-30)).Sum(w => w.TotalDistance());
    public bool IsActive() => Ailments.Any(a => !a.IsRecovered);
    public bool IsInactive() => !IsActive();
    public DateTime GetLastWorkoutDate() => Workouts.OrderBy(w => w.Date).Last().Date;
    public int TotalWorkoutsCompleted() => Workouts.Count;
    public List<Ailment> GetActiveAilments() => [.. Ailments.Where(a => !a.IsRecovered)];
    public List<Workout> WorkoutsPendingFeedback() => [.. Workouts.Where(w => w.HasLinkedSession() && !w.HasFeedback())];
    public List<Workout> WorkoutsWithFeedback() => [.. Workouts.Where(w => w.HasLinkedSession() && w.HasFeedback())];
    public List<Workout> WorkoutsThisWeek() => [.. Workouts.Where(w => w.Date > DateTime.Now.AddDays(-7))];
    public Workout GetWorkoutById(int workoutId) => Workouts.First(w => w.Id == workoutId);
    public int Age() => DateTime.Now.Year - BirthDate.Year;
    public int YearsOfExperience() => DateTime.Now.Year - DateStartedRunning.Year;
    public bool HasCompleted(Session s) => Workouts.Any(w => w.HasLinkedSession() && w.Session == s);
    public List<Workout> WorkoutsFromPastDays(int lookbackInDays) => [.. Workouts.Where(w => w.Date > DateTime.Now.AddDays(-lookbackInDays))];
    public double HeartRateReserveFraction(int heartRate) => (heartRate - MinHeartRate) / (MaxHeartRate - MinHeartRate);

}

