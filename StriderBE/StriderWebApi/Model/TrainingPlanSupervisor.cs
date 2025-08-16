using StriderWebApi.Domain.Enums;
namespace StriderWebApi.Model;

public abstract class ITrainingPlanAnalyzer
{
    public abstract List<Notification> Analyze(TrainingPlan trainingPlan, Athlete athlete);

    public abstract List<Metric<dynamic>> GetMetrics(TrainingPlan trainingPlan, Athlete athlete);

    public Metric<T> GetMetricByName<T>(List<Metric<dynamic>> metrics, String name) {
        Metric<dynamic> metric = metrics.First(m => m.Name.Equals(name));
        return new Metric<T> { Name = name, Value = (T)metric.Value };
    }

}

public class Metric<T>
{
    public required string Name { get; set; }
    public required T Value { get; set; }
}

public class TrainingLoadCalculator
{
    public double CalculateExponentialAverageTrainingLoad(Athlete athlete, int days)
    {
        List<Workout> workouts = athlete.WorkoutsFromPastDays(days);
        double weightFactor = Math.Exp(-1 / days);

        double totalLoad = 0;
        foreach (var workout in workouts)
        {
            totalLoad += athlete.TrainingLoadFrom(workout) * Math.Pow(weightFactor, DateTime.Now.Subtract(workout.Date).Days);
        }
        double normalization = workouts.Sum(w => Math.Pow(weightFactor, DateTime.Now.Subtract(w.Date).Days));
        return totalLoad / normalization;
    }

    public double CalculateAverageTrainingLoad(Athlete athlete, int days) => athlete.WorkoutsFromPastDays(days).Average(athlete.TrainingLoadFrom);
    public double CalculateStandardDeviationOfTrainingLoad(Athlete athlete, int days) => Math.Sqrt(athlete.WorkoutsFromPastDays(days).Select(athlete.TrainingLoadFrom).Average(l => Math.Pow(l - athlete.WorkoutsFromPastDays(days).Average(athlete.TrainingLoadFrom), 2)));
    public double CalculateTotalTrainingLoad(Athlete athlete, int days) => athlete.WorkoutsFromPastDays(days).Sum(athlete.TrainingLoadFrom);
    public double CalculateTrainingLoadBetween(Athlete athlete, DateTime startDate, DateTime endDate) => athlete.Workouts.Where(w => w.Date > startDate && w.Date < endDate).Sum(athlete.TrainingLoadFrom);
    public double CalculateStandardDeviationOfTrainingLoadBetween(Athlete athlete, DateTime startDate, DateTime endDate) => athlete.Workouts.Where(w => w.Date > startDate && w.Date < endDate).Select(athlete.TrainingLoadFrom).Average(l => Math.Pow(l - athlete.Workouts.Where(w => w.Date > startDate && w.Date < endDate).Average(athlete.TrainingLoadFrom), 2));
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

            notifications.Add(new Notification()
            {
                Title = string.Format("Athlete {0} has {1} incomplete workouts during the last week", athlete.Name, incompletedWorkouts),
                Message = string.Format(
                    "The following workouts were not completed:\n\n{0}\n\n" +
                    "This may require adjusting the training plan or addressing any barriers the athlete is facing.",
                    sessionDetails),
                CreatedBy = "Incompleted Workouts Analyzer",
                Type = NotificationType.Warning
            });
        }

        return notifications;
    }
    
    public override List<Metric<dynamic>> GetMetrics(TrainingPlan trainingPlan, Athlete athlete)
    {
        return new List<Metric<dynamic>>()
        {
            new() { Name = "Incompleted Workouts Last Week", Value = trainingPlan.Sessions.Where(s => !athlete.HasCompleted(s)).Count() }
        };
    }

}

public class InjuryPreventionAnalyzer : ITrainingPlanAnalyzer
{

    public int MildDiscomfortsThreshold { get; set; } = 3;
    public int ModerateDiscomfortsThreshold { get; set; } = 2;
    public int SevereDiscomfortsThreshold { get; set; } = 1;

    public string DISCOMFORTS_LAST_WEEK_TITLE = "Discomforts Last Week";
    public string MILD_DISCOMFORTS_LAST_WEEK_TITLE = "Mild Discomforts Last Week";
    public string MODERATE_DISCOMFORTS_LAST_WEEK_TITLE = "Moderate Discomforts Last Week";
    public string SEVERE_DISCOMFORTS_LAST_WEEK_TITLE = "Severe Discomforts Last Week";


    public override List<Notification> Analyze(TrainingPlan trainingPlan, Athlete athlete)
    {
        List<Notification> notifications = new();
        List<Metric<dynamic>> metrics = GetMetrics(trainingPlan, athlete);

        if (GetMetricByName<int>(metrics, DISCOMFORTS_LAST_WEEK_TITLE).Value > 0)
        {

            Metric<int> mildDiscomfortsMetric = GetMetricByName<int>(metrics, MILD_DISCOMFORTS_LAST_WEEK_TITLE);
            Metric<int> moderateDiscomfortsMetric = GetMetricByName<int>(metrics, MODERATE_DISCOMFORTS_LAST_WEEK_TITLE);
            Metric<int> severeDiscomfortsMetric = GetMetricByName<int>(metrics, SEVERE_DISCOMFORTS_LAST_WEEK_TITLE);

            Dictionary<BodyPart, int> mildDiscomfortsByBodyPart = GetMildDiscomfortsByBodyPartThatAre(trainingPlan, athlete, DiscomfortLevel.Mild);
            Dictionary<BodyPart, int> moderateDiscomfortsByBodyPart = GetMildDiscomfortsByBodyPartThatAre(trainingPlan, athlete, DiscomfortLevel.Moderate);
            Dictionary<BodyPart, int> severeDiscomfortsByBodyPart = GetMildDiscomfortsByBodyPartThatAre(trainingPlan, athlete, DiscomfortLevel.Severe);

            if (mildDiscomfortsMetric.Value > MildDiscomfortsThreshold || moderateDiscomfortsMetric.Value > ModerateDiscomfortsThreshold || severeDiscomfortsMetric.Value > SevereDiscomfortsThreshold)
            {
                notifications.Add(new Notification()
                {
                    Title = string.Format("Athlete {0} has too many discomforts during the last week", athlete.Name),
                    Message = string.Format(
                        "I detected the following discomforts:\n\n" +
                        "🔴 SEVERE Discomforts ({0} total):\n{1}\n\n" +
                        "🟡 MODERATE Discomforts ({2} total):\n{3}\n\n" +
                        "🟢 MILD Discomforts ({4} total):\n{5}" +
                        "",
                        severeDiscomfortsMetric.Value,
                        string.Join("\n", severeDiscomfortsByBodyPart.Where(kvp => kvp.Value > 0).Select(kvp => $"   • {kvp.Key}: {kvp.Value}")),
                        string.Join("\n", moderateDiscomfortsByBodyPart.Where(kvp => kvp.Value > 0).Select(kvp => $"   • {kvp.Key}: {kvp.Value}")),
                        mildDiscomfortsMetric.Value,
                        string.Join("\n", mildDiscomfortsByBodyPart.Where(kvp => kvp.Value > 0).Select(kvp => $"   • {kvp.Key}: {kvp.Value}"))
                    ),
                    CreatedBy = "Injury Prevention Analyzer",
                    Type = NotificationType.Warning
                });
            }
        }

        return notifications;
    }

    public override List<Metric<dynamic>> GetMetrics(TrainingPlan trainingPlan, Athlete athlete)
    {

        return new List<Metric<dynamic>>()
        {
            new() { Name = DISCOMFORTS_LAST_WEEK_TITLE, Value = GetDiscomfortsInLastWeek(trainingPlan, athlete).Count() },
            new() { Name = MILD_DISCOMFORTS_LAST_WEEK_TITLE, Value = GetDiscomfortsInLastWeek(trainingPlan, athlete).FindAll(d => d.Level == DiscomfortLevel.Mild).Count() },
            new() { Name = MODERATE_DISCOMFORTS_LAST_WEEK_TITLE, Value = GetDiscomfortsInLastWeek(trainingPlan, athlete).FindAll(d => d.Level == DiscomfortLevel.Moderate).Count() },
            new() { Name = SEVERE_DISCOMFORTS_LAST_WEEK_TITLE, Value = GetDiscomfortsInLastWeek(trainingPlan, athlete).FindAll(d => d.Level == DiscomfortLevel.Severe).Count() }
        };

    }

    private List<Discomfort> GetDiscomfortsInLastWeek(TrainingPlan trainingPlan, Athlete athlete)
    {
        return athlete.WorkoutsThisWeek().FindAll(w => w.HasDiscomfort()).SelectMany(w => w.Discomforts).ToList();
    }

    private Dictionary<BodyPart, int> GetMildDiscomfortsByBodyPartThatAre(TrainingPlan trainingPlan, Athlete athlete, DiscomfortLevel discomfortLevel)
    {
        List<Discomfort> discomfortsInLastWeek = GetDiscomfortsInLastWeek(trainingPlan, athlete);

        Dictionary<BodyPart, int> discomfortsByBodyPart = new();

        foreach (var bodyPart in Enum.GetValues<BodyPart>())
        {
            discomfortsByBodyPart.Add(bodyPart, discomfortsInLastWeek.FindAll(d => d.Level == discomfortLevel && d.BodyPart.Equals(bodyPart)).Count());
        }
        return discomfortsByBodyPart;
    }

}


public class FatigueAnalyzer : ITrainingPlanAnalyzer
{
    public int AcuteLookBackInDays = 7;
    public int FatigueThreshold = 75;
    private readonly TrainingLoadCalculator _calculator = new();

    private readonly string ACUTE_TRAINING_LOAD_TITLE = "Acute Training Load";

    public override List<Notification> Analyze(TrainingPlan trainingPlan, Athlete athlete)
    {
        List<Notification> notifications = new();
        double acuteTrainingLoad = _calculator.CalculateExponentialAverageTrainingLoad(athlete, AcuteLookBackInDays);

        if (acuteTrainingLoad > FatigueThreshold)
        {
            notifications.Add(new Notification()
            {
                Title = string.Format("Athlete {0} may be fatigued", athlete.Name),
                Message = string.Format("The athlete's acute training load is {0} averaged over the last {1} days. Set limit was {2} over the same period. Athlete may be fatigued.", acuteTrainingLoad, AcuteLookBackInDays, FatigueThreshold),
                CreatedBy = "Fatigue Analyzer",
                Type = NotificationType.Warning
            });
        }

        return notifications;
    }

    public override List<Metric<dynamic>> GetMetrics(TrainingPlan trainingPlan, Athlete athlete)
    {
        return new List<Metric<dynamic>>()
        {
            new() { Name = ACUTE_TRAINING_LOAD_TITLE + " (Last " + AcuteLookBackInDays + " days)", Value = (int)(_calculator.CalculateExponentialAverageTrainingLoad(athlete, AcuteLookBackInDays) / athlete.MaxTrainingLoad() * 100) }
        };
    }
}

public class LoadBalanceAnalyzer : ITrainingPlanAnalyzer
{
    public int AcuteLookBackInDays = 7;
    public int ChronicLookBackInDays = 42;
    public double UndertrainmentThreshold = 0.8;
    public double OverreachThreshold = 1.3;
    public double OverTrainingThreshold = 1.6;
    private readonly TrainingLoadCalculator _calculator = new();

    private readonly string ACUTE_CHRONIC_RATIO_TITLE = "Acute/Chronic Ratio";

    public override List<Notification> Analyze(TrainingPlan trainingPlan, Athlete athlete)
    {
        List<Notification> notifications = new();
        double chronicTrainingLoad = _calculator.CalculateExponentialAverageTrainingLoad(athlete, ChronicLookBackInDays);
        double acRatio = _calculator.CalculateExponentialAverageTrainingLoad(athlete, AcuteLookBackInDays) / chronicTrainingLoad;

        if (acRatio < UndertrainmentThreshold)
        {
            notifications.Add(new Notification()
            {
                Title = string.Format("Athlete {0} may be undertrained", athlete.Name),
                Message = string.Format("The athlete's acute-chronic ratio is {0}. Set minimum limit for undertraining was {1}%. Athlete may be undertraining.", acRatio, UndertrainmentThreshold),
                CreatedBy = "Load Balance Analyzer",
                Type = NotificationType.Warning
            });
        }

        if (acRatio > OverreachThreshold && acRatio < OverTrainingThreshold)
        {
            notifications.Add(new Notification()
            {
                Title = string.Format("Athlete {0} may be overreaching", athlete.Name),
                Message = string.Format("The athlete's acute-chronic ratio is {0}. Set limits for overreach were {1}% and {2}%. Athlete may be overreaching.", acRatio, OverreachThreshold, OverTrainingThreshold),
                CreatedBy = "Load Balance Analyzer",
                Type = NotificationType.Warning
            });
        }

        if (acRatio > OverTrainingThreshold)
        {
            notifications.Add(new Notification()
            {
                Title = string.Format("Athlete {0} may be overtrained", athlete.Name),
                Message = string.Format("The athlete's acute-chronic ratio is {0}. Set limit for overtraining was {1}%. Athlete may be overtraining.", acRatio, OverTrainingThreshold),
                CreatedBy = "Load Balance Analyzer",
                Type = NotificationType.Warning
            });
        }

        return notifications;
    }

    public override List<Metric<dynamic>> GetMetrics(TrainingPlan trainingPlan, Athlete athlete)
    {
        return new List<Metric<dynamic>>()
        {
            new() { Name = ACUTE_CHRONIC_RATIO_TITLE, Value = _calculator.CalculateExponentialAverageTrainingLoad(athlete, AcuteLookBackInDays) / _calculator.CalculateExponentialAverageTrainingLoad(athlete, ChronicLookBackInDays) }
        };
    }
}

public class StressBalanceAnalyzer : ITrainingPlanAnalyzer
{
    public int AcuteLookBackInDays = 7;
    public int ChronicLookBackInDays = 42;
    public double StressBalanceThreshold = 15;
    public int CompetitionLookForwardInDays = 5;
    private readonly TrainingLoadCalculator _calculator = new();

    private readonly string STRESS_BALANCE_TITLE = "Stress Balance";

    public override List<Notification> Analyze(TrainingPlan trainingPlan, Athlete athlete)
    {
        List<Notification> notifications = new();

        if (trainingPlan.AnyCompetitionsIn(athlete, CompetitionLookForwardInDays))
        {
            DateTime nextCompetition = trainingPlan.NextCompetitionIn(athlete, CompetitionLookForwardInDays).Date;
            double stressBalance =  calculateTrainingStressBalance(athlete);

            if (stressBalance < StressBalanceThreshold)
            {
                notifications.Add(new Notification()
                {
                    Title = string.Format("Athlete {0} may be overloaded for competitions", athlete.Name),
                    Message = string.Format("The athlete's stress balance is {0}. Set limit was {1} for competitions. Athlete may be overloaded for competition in the next {2} days.", stressBalance, StressBalanceThreshold, nextCompetition.Subtract(DateTime.Now).Days),
                    CreatedBy = "Stress Balance Analyzer",
                    Type = NotificationType.Warning
                });
            }
        }

        return notifications;
    }

    public override List<Metric<dynamic>> GetMetrics(TrainingPlan trainingPlan, Athlete athlete)
    {
        return new List<Metric<dynamic>>()
        {
            new() { Name = STRESS_BALANCE_TITLE, Value = calculateTrainingStressBalance(athlete) }
        };
    }

    private double calculateTrainingStressBalance(Athlete athlete)
    {
        return _calculator.CalculateExponentialAverageTrainingLoad(athlete, AcuteLookBackInDays) - _calculator.CalculateExponentialAverageTrainingLoad(athlete, ChronicLookBackInDays);
    }

}

public class MonotonyAnalyzer : ITrainingPlanAnalyzer
{
    public double MaxMonotonyThreshold = 0.67;
    public double StrainProbabilityThreshold = 0.8;

    private TrainingLoadCalculator _calculator = new();

    private readonly string MONOTONY_TITLE = "Monotony";
    private readonly string STRAIN_TITLE = "Strain (Distribution Probability)";

    public override List<Metric<dynamic>> GetMetrics(TrainingPlan trainingPlan, Athlete athlete)
    {
        return new List<Metric<dynamic>>()
        {
            new() { Name = MONOTONY_TITLE, Value = MonotonyInWeek(athlete, DateTime.Now.AddDays(-7), DateTime.Now) },
            new() { Name = STRAIN_TITLE, Value = StrainProbability(athlete, StrainInWeek(athlete, DateTime.Now.AddDays(-7), DateTime.Now)) }
        };
    }

    public List<double> Strains(Athlete athlete)
    {

        DateTime firstWorkout = athlete.Workouts.Min(w => w.Date);
        DateTime date = DateTime.Now;

        List<double> strains = new();
        while (date > firstWorkout)
        {
            strains.Add(StrainInWeek(athlete, date.AddDays(-7), date));
            date = date.AddDays(-7);
        }

        return strains;

    }

    public override List<Notification> Analyze(TrainingPlan trainingPlan, Athlete athlete)
    {
        List<Notification> notifications = new();
        double monotony = MonotonyInWeek(athlete, DateTime.Now.AddDays(-7), DateTime.Now);
        double strain = StrainInWeek(athlete, DateTime.Now.AddDays(-7), DateTime.Now);
        double strainProbability = StrainProbability(athlete, strain);

        if (monotony > MaxMonotonyThreshold)
        {
            notifications.Add(new Notification()
            {
                Title = string.Format("Athlete {0} has excessively varied training (low monotony)", athlete.Name),
                Message = string.Format("The athlete's monotony is {0}. Set upper limit was {1}. Plan may be too varied.", monotony, MaxMonotonyThreshold),
                CreatedBy = "Monotony Analyzer",
                Type = NotificationType.Warning
            });
        }

        if (strainProbability > StrainProbabilityThreshold)
        {
            notifications.Add(new Notification()
            {
                Title = string.Format("Athlete {0} has excessively high strain", athlete.Name),
                Message = string.Format("The athlete's strain is in the top {0}% of all strains. Set upper limit was {1}%. Plan may be straining the athlete too much.", monotony, MaxMonotonyThreshold),
                CreatedBy = "Monotony Analyzer",
                Type = NotificationType.Warning
            });
        }


        return notifications;

    }

    private double StrainProbability(Athlete athlete, double strain)
    {
        List<double> strains = Strains(athlete);
        double strainProbability =  (double)strains.Where(s => s > strain).ToList().Count / strains.Count;
        return strainProbability;
    }

    public double MonotonyInWeek(Athlete athlete, DateTime start, DateTime end)
    {
        double mean = _calculator.CalculateTrainingLoadBetween(athlete, start, end) / (end.Subtract(start).Days);
        double stdev = _calculator.CalculateStandardDeviationOfTrainingLoadBetween(athlete, start, end);
        return mean / (mean + stdev);
    }

    public double StrainInWeek(Athlete athlete, DateTime start, DateTime end)
    {
        return MonotonyInWeek(athlete, start, end)  * _calculator.CalculateTrainingLoadBetween(athlete, start, end) * 2;
    }
    

}

    





