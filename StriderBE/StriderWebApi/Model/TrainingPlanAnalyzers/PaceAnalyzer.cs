using StriderWebApi.Domain.Enums;
namespace StriderWebApi.Model;

public class PaceAnalyzer : ITrainingPlanAnalyzer
{

	public int AveragePaceDifferencePerThousandMetersInSecondsThreshold = 5;
	public int MaxPaceDifferencePerThousandMetersInSecondsThreshold = 10;
	public int LookBackInDays = 3;
	private WorkoutAnalyzer _analyzer = new();

	public override List<Notification> Analyze(TrainingPlan trainingPlan, Athlete athlete)
	{

		List<Notification> notifications = new();
		Dictionary<Workout, int> averagePaceDifferences = new();
		Dictionary<Workout, int> maxPaceDifferences = new();

		if (athlete.WorkoutsFromPastDays(LookBackInDays).Where(w => w.HasLinkedSession()).ToList().Count > 0)
		{

			foreach (var workout in athlete.WorkoutsFromPastDays(LookBackInDays))
			{
				Analysis analysis = _analyzer.Analyze(athlete, workout, workout.Session!);
				if (workout.HasLinkedSession())
				{
					averagePaceDifferences.Add(workout, analysis.AveragePaceDifferencePerThousandMetersInSeconds());
					maxPaceDifferences.Add(workout, analysis.MaxPaceDifferencePerThousandMetersInSeconds());
				}
			}

			if (averagePaceDifferences.Values.Max() > AveragePaceDifferencePerThousandMetersInSecondsThreshold)
			{
				Dictionary<Workout, int> worstWorkouts = averagePaceDifferences.Where(kvp => kvp.Value > AveragePaceDifferencePerThousandMetersInSecondsThreshold).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
				notifications.Add(new Notification()
				{
					Title = string.Format("Athlete {0} has high pace differences compared to expected", athlete.Name),
					Message = string.Format("The following workouts in the last {0} days have noticably high pace differences compared to expected:\n\n" +
					string.Join("\n", worstWorkouts.Select(w => string.Format("{0} ({1}): {2} s/km", w.Key.Name, w.Key.Date.ToShortDateString(), w.Value))), LookBackInDays),
					CreatedBy = "Pace Analyzer",
					Type = NotificationType.Warning
				});
			}

			if (maxPaceDifferences.Values.Max() > MaxPaceDifferencePerThousandMetersInSecondsThreshold)
			{
				Dictionary<Workout, int> worstWorkouts = maxPaceDifferences.Where(kvp => kvp.Value > MaxPaceDifferencePerThousandMetersInSecondsThreshold).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
				notifications.Add(new Notification()
				{
					Title = string.Format("Athlete {0} has high pace differences compared to expected", athlete.Name),
					Message = string.Format("The following workouts in the last {0} days had laps with very high pace differences:\n\n" +
					string.Join("\n", worstWorkouts.Select(w => string.Format("{0} ({1}): {2} s/km", w.Key.Name, w.Key.Date.ToShortDateString(), w.Value))), LookBackInDays),
					CreatedBy = "Pace Analyzer",
					Type = NotificationType.Warning
				});
			}

		}

		return notifications;

	}



	public override List<Metric<dynamic>> GetMetrics(TrainingPlan trainingPlan, Athlete athlete)
	{
		return [];
	}
}



