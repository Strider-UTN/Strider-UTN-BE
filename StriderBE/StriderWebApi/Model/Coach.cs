using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Model;

public class Coach : User
{
    public List<Team> Teams { get; set; } = [];
    public List<TrainingPlan> TrainingPlans { get; set; } = [];
    public List<Session> Templates { get; set; } = [];
    public List<Athlete> Athletes { get; set; } = [];

    public void AddTeam(Team team) => Teams.Add(team);
    public void RemoveTeam(Team team) => Teams.Remove(team);
    public void AddTrainingPlan(TrainingPlan trainingPlan) => TrainingPlans.Add(trainingPlan);
    public void RemoveTrainingPlan(TrainingPlan trainingPlan) => TrainingPlans.Remove(trainingPlan);
    public void AddTemplate(Session template) => Templates.Add(template);
    public void AddAthlete(Athlete athlete) => Athletes.Add(athlete);
    
    public int TotalIndividualAthletes() => Athletes.Count;
    public int ActiveIndividualAthletes() => Athletes.FindAll(a => a.IsActive()).Count;
    public int InactiveIndividualAthletes() => Athletes.FindAll(a => a.IsInactive()).Count;
    public int ActiveTeams() => Teams.FindAll(t => t.IsActive).Count;
    public int NewTeamsSinceLastMonth() => Teams.FindAll(t => t.CreatedLastMonth(DateTime.Today)).Count;
    public int AthletesInTeams() => Teams.Sum(t => t.TotalAthletes());
    public int TotalTrainingLocations() => Teams.Sum(t => t.TotalTrainingLocations());
    public int TotalWorkoutsCompletedByIndividualAthletes() => Athletes.Sum(a => a.TotalWorkoutsCompleted());
}