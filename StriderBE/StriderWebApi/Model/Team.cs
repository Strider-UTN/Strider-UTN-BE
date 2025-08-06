namespace StriderWebApi.Model;

public class Team
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; }
    public bool IsPublic { get; set; }
    public bool AutomaticInscription { get; set; }
    public bool RequiresManualApproval { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
    
    public List<TrainingLocation> TrainingLocations { get; set; } = [];
    public List<Athlete> Athletes { get; set; } = [];
    public List<Coach> Coaches { get; set; } = [];

    public void AddAthlete(Athlete athlete) => Athletes.Add(athlete);
    public void AddCoach(Coach coach) => Coaches.Add(coach);
    
    public bool CreatedLastMonth(DateTime today)
    {
        return CreationDate.Month == today.Month && CreationDate.Year == today.Year;
    }

    public int TotalAthletes()
    {
        return Athletes.Count;
    }

    public int TotalTrainingLocations()
    {
        return TrainingLocations.Count;
    }
}