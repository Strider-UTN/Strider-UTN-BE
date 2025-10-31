using System;
using System.Linq;
using System.Collections.Generic;
using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Model;

public class IncompletedWorkoutsAnalyzer : ITrainingPlanAnalyzer
{
    public int IncompletedWorkoutsThreshold { get; set; }

    public override List<Notification> Analyze(TrainingPlan trainingPlan, Athlete athlete)
    {
        List<Notification> notifications = new();

        var incompletedSessions = trainingPlan.Sessions
            .Where(s => s.Date < DateTime.Now.AddDays(-1) &&
                        s.Date > DateTime.Now.AddDays(-7) &&
                        s.HasAthlete(athlete) &&
                        !athlete.HasCompleted(s))
            .OrderBy(s => s.Date)
            .ToList();

        int incompletedWorkouts = incompletedSessions.Count;

        if (incompletedWorkouts > IncompletedWorkoutsThreshold)
        {
            string sessionDetails = string.Join("\n", incompletedSessions.Select(s =>
                $"   • {s.Date:MMM dd, yyyy} - {s.Name ?? "Unnamed Session"}"));

            notifications.Add(new Notification
            {
                Title = $"Athlete {athlete.Name} has {incompletedWorkouts} incomplete workouts during the last week",
                Message =
                    $"The following workouts were not completed:\n\n{sessionDetails}\n\n" +
                    "This may require adjusting the training plan or addressing any barriers the athlete is facing.",
                CreatedBy = "Incompleted Workouts Analyzer",
                Type = NotificationType.Warning
            });
        }

        return notifications;
    }

    public override List<Metric<dynamic>> GetMetrics(TrainingPlan trainingPlan, Athlete athlete)
    {
        return new List<Metric<dynamic>>
        {
            new()
            {
                Name = "Incompleted Workouts Last Week",
                Value = trainingPlan.Sessions.Count(s => !athlete.HasCompleted(s))
            }
        };
    }
}
