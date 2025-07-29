namespace StriderWebApi.Model;

public class Coach(User user)
{
    
    public User User { get; } = user;

    public List<Team> Teams { get; } = [];

    public List<TrainingPlan> TrainingPlans { get; } = [];

    public List<Session> Templates { get; } = [];

    public void AddTeam(Team team) => Teams.Add(team);

    public void AddTrainingPlan(TrainingPlan trainingPlan) => TrainingPlans.Add(trainingPlan);

    public void RemoveTrainingPlan(TrainingPlan trainingPlan) => TrainingPlans.Remove(trainingPlan);

    public void AddTemplate(Session template) => Templates.Add(template);

}