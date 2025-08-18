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


