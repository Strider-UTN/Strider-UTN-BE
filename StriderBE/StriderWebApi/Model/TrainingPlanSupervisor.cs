using StriderWebApi.Domain.Enums;
namespace StriderWebApi.Model;

public class Metric<T>
{
    public required string Name { get; set; }
    public required T Value { get; set; }
}

public class TrainingPlanSupervisor
{
    public List<ITrainingPlanAnalyzer> Analyzers { get; set; } = [];
    public required TrainingPlan TrainingPlan { get; set; }

    public List<Notification> Analyze(Athlete athlete)
    {
        List<Notification> notifications = new();
        foreach (var supervisor in Analyzers)
        {
            List<Notification> analyzerNotifications = supervisor.Analyze(TrainingPlan, athlete);
            notifications.AddRange(analyzerNotifications);
        }
        return notifications;
    }
}

