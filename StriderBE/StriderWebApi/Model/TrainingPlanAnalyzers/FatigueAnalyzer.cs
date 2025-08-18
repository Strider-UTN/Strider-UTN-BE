using StriderWebApi.Domain.Enums;
namespace StriderWebApi.Model;

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



