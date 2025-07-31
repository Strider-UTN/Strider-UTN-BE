namespace StriderWebApi.Model;

public class Coach(int id, string username, string name, string password, string email, Gender gender, string address) : User(id, username, name, password, email, gender, address)
{
    public List<Team> Teams { get; } = [];

    public List<TrainingPlan> TrainingPlans { get; } = [];

    public List<Session> Templates { get; } = [];

    public void AddTeam(Team team) => Teams.Add(team);

    public void AddTrainingPlan(TrainingPlan trainingPlan) => TrainingPlans.Add(trainingPlan);

    public void RemoveTrainingPlan(TrainingPlan trainingPlan) => TrainingPlans.Remove(trainingPlan);

    public void AddTemplate(Session template) => Templates.Add(template);

}