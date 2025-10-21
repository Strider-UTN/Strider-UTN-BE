using StriderWebApi.Domain.Enums;
namespace StriderWebApi.Model;

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
				Message = string.Format("The athlete's current weekly strain is in the top {0}% of all strains. Set upper limit was {1}%. Plan may be straining the athlete too much.", monotony, MaxMonotonyThreshold),
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
		double mean = _calculator.CalculateTrainingLoadBetween(athlete, start, end) / end.Subtract(start).Days;
		double stdev = _calculator.CalculateStandardDeviationOfTrainingLoadBetween(athlete, start, end);
		return mean / (mean + stdev);
	}

	public double StrainInWeek(Athlete athlete, DateTime start, DateTime end)
	{
		return MonotonyInWeek(athlete, start, end)  * _calculator.CalculateTrainingLoadBetween(athlete, start, end) * 2;
	}
	

}



