using StriderWebApi.Domain.Enums;
namespace StriderWebApi.Model;

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
				Message = string.Format("The athlete's acute-chronic ratio is {0}. Set minimum limit for undertraining was {1}. Athlete may be undertraining.", acRatio, UndertrainmentThreshold),
				CreatedBy = "Load Balance Analyzer",
				Type = NotificationType.Warning
			});
		}

		if (acRatio > OverreachThreshold && acRatio < OverTrainingThreshold)
		{
			notifications.Add(new Notification()
			{
				Title = string.Format("Athlete {0} may be overreaching", athlete.Name),
				Message = string.Format("The athlete's acute-chronic ratio is {0}. Set limits for overreach were {1} and {2}. Athlete may be overreaching.", acRatio, OverreachThreshold, OverTrainingThreshold),
				CreatedBy = "Load Balance Analyzer",
				Type = NotificationType.Warning
			});
		}

		if (acRatio > OverTrainingThreshold)
		{
			notifications.Add(new Notification()
			{
				Title = string.Format("Athlete {0} may be overtrained", athlete.Name),
				Message = string.Format("The athlete's acute-chronic ratio is {0}. Set limit for overtraining was {1}. Athlete may be overtraining.", acRatio, OverTrainingThreshold),
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



