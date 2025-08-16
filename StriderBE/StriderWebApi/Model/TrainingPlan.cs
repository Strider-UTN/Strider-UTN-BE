namespace StriderWebApi.Model;

public class TrainingPlan
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
    public List<Session> Sessions { get; set; } = [];
    public void AddSession(Session session) => Sessions.Add(session);
    public void RemoveSession(Session session) => Sessions.Remove(session);
    public bool AnyCompetitionsIn(Athlete athlete, int competitionLookForwardInDays) => Sessions.Any(s => s.HasAthlete(athlete) && s.SessionType == Domain.Enums.WorkoutType.COMPETITION && s.Date < DateTime.Now.AddDays(competitionLookForwardInDays));
    public Session NextCompetitionIn(Athlete athlete, int competitionLookForwardInDays) => Sessions.Where(s => s.HasAthlete(athlete) && s.SessionType == Domain.Enums.WorkoutType.COMPETITION).OrderBy(s => s.Date).First();
}