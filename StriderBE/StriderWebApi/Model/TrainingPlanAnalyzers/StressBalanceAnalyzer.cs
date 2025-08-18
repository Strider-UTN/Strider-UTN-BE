using StriderWebApi.Domain.Enums;
namespace StriderWebApi.Model;

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
			double stressBalance =  CalculateTrainingStressBalance(athlete);

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
			new() { Name = STRESS_BALANCE_TITLE, Value = CalculateTrainingStressBalance(athlete) }
		};
	}

	private double CalculateTrainingStressBalance(Athlete athlete)
	{
		return _calculator.CalculateExponentialAverageTrainingLoad(athlete, AcuteLookBackInDays) - _calculator.CalculateExponentialAverageTrainingLoad(athlete, ChronicLookBackInDays);
	}

}



