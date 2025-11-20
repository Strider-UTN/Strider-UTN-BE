using StriderWebApi.Domain.Enums;
using StriderWebApi.Domain.DomainClasses;

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
    public string? VO2Max { get; set; } // Velocidad máxima por km en formato mm:ss (ejemplo: "03:30")
    public int MinHeartRate { get; set; }
    public int MaxHeartRate { get; set; }
    public int ThresholdHeartRate { get; set; }
    public List<string> MedicalConditions { get; set; } = [];
    public List<string> Objectives { get; set; } = [];
    public List<AthleteInjury> Injuries { get; set; } = [];
    public List<Workout> Workouts { get; set; } = [];
    public void AddWorkouts(List<Workout> workouts) => Workouts.AddRange(workouts);
    public void UpdateVO2Max(string vO2Max) => VO2Max = vO2Max;
    
    /// <summary>
    /// Calcula la velocidad basada en un porcentaje del VO2Max.
    /// Retorna los segundos totales por km (ejemplo: 210 para 03:30).
    /// </summary>
    public double Speed(int percentage)
    {
        if (string.IsNullOrWhiteSpace(VO2Max))
            return 0;
        
        // Parsear formato mm:ss a segundos totales
        var parts = VO2Max.Split(':');
        if (parts.Length != 2 || !int.TryParse(parts[0], out var minutes) || !int.TryParse(parts[1], out var seconds))
            return 0;
        
        var totalSeconds = minutes * 60 + seconds;
        
        // Aplicar porcentaje (si el porcentaje es 100, retorna el mismo tiempo)
        // Si el porcentaje es menor, el tiempo aumenta (más lento)
        // Si el porcentaje es mayor, el tiempo disminuye (más rápido)
        return totalSeconds * 100.0 / percentage;
    }
    
    public void AddMedicalCondition(string medicalCondition) => MedicalConditions.Add(medicalCondition);
    public double WeeklyDistance() => Workouts.Where(w => w.Date > DateTime.Now.AddDays(-7)).Sum(w => w.TotalDistance());
    public double MonthlyDistance() => Workouts.Where(w => w.Date > DateTime.Now.AddDays(-30)).Sum(w => w.TotalDistance());
    public bool IsActive() => Injuries.Any(i => i.Status != InjuryStatus.Recovered);
    public bool IsInactive() => !IsActive();
    public DateTime GetLastWorkoutDate() => Workouts.OrderBy(w => w.Date).Last().Date;
    public int TotalWorkoutsCompleted() => Workouts.Count;
    public List<AthleteInjury> GetActiveInjuries() => [.. Injuries.Where(i => i.Status != InjuryStatus.Recovered)];
    public List<Workout> WorkoutsPendingFeedback() => [.. Workouts.Where(w => w.HasLinkedSession() && !w.HasFeedback())];
    public List<Workout> WorkoutsWithFeedback() => [.. Workouts.Where(w => w.HasLinkedSession() && w.HasFeedback())];
    public List<Workout> WorkoutsThisWeek() => [.. Workouts.Where(w => w.Date > DateTime.Now.AddDays(-7))];
    public Workout GetWorkoutById(int workoutId) => Workouts.First(w => w.Id == workoutId);
    public int Age() => DateTime.Now.Year - BirthDate.Year;
    public int YearsOfExperience() => DateTime.Now.Year - DateStartedRunning.Year;
    public bool HasCompleted(Session s) => Workouts.Any(w => w.HasLinkedSession() && w.Session == s);
    public List<Workout> WorkoutsFromPastDays(int lookbackInDays) => [.. Workouts.Where(w => w.Date > DateTime.Now.AddDays(-lookbackInDays))];
    public double HeartRateReserveFraction(double heartRate) => (heartRate - MinHeartRate) / (MaxHeartRate - MinHeartRate);

}

