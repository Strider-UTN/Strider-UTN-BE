using StriderWebApi.Domain.Enums;
namespace StriderWebApi.Model;

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



