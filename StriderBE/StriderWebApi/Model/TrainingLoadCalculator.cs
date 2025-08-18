namespace StriderWebApi.Model;

public class TrainingLoadCalculator
{
    public double CalculateExponentialAverageTrainingLoad(Athlete athlete, int days)
    {
        List<Workout> workouts = athlete.WorkoutsFromPastDays(days);
        double weightFactor = Math.Exp(-1 / days);

        double totalLoad = 0;
        foreach (var workout in workouts)
        {
            totalLoad += TrainingLoadFrom(athlete, workout) * Math.Pow(weightFactor, DateTime.Now.Subtract(workout.Date).Days);
        }
        double normalization = workouts.Sum(w => Math.Pow(weightFactor, DateTime.Now.Subtract(w.Date).Days));
        return totalLoad / normalization;
    }

    public double CalculateAverageTrainingLoad(Athlete athlete, int days) => athlete.WorkoutsFromPastDays(days).Average(w => TrainingLoadFrom(athlete, w));
    public double CalculateStandardDeviationOfTrainingLoad(Athlete athlete, int days) => Math.Sqrt(athlete.WorkoutsFromPastDays(days).Select(w => TrainingLoadFrom(athlete, w)).Average(l => Math.Pow(l - athlete.WorkoutsFromPastDays(days).Average(w => TrainingLoadFrom(athlete, w)), 2)));
    public double CalculateTotalTrainingLoad(Athlete athlete, int days) => athlete.WorkoutsFromPastDays(days).Sum(w => TrainingLoadFrom(athlete, w));
    public double CalculateTrainingLoadBetween(Athlete athlete, DateTime startDate, DateTime endDate) => athlete.Workouts.Where(w => w.Date > startDate && w.Date < endDate).Sum(w => TrainingLoadFrom(athlete, w));
    public double CalculateStandardDeviationOfTrainingLoadBetween(Athlete athlete, DateTime startDate, DateTime endDate) => athlete.Workouts.Where(w => w.Date > startDate && w.Date < endDate).Select(w => TrainingLoadFrom(athlete, w)).Average(l => Math.Pow(l - athlete.Workouts.Where(w => w.Date > startDate && w.Date < endDate).Average(w => TrainingLoadFrom(athlete, w)), 2));
    private static double TrainingLoadFrom(Athlete athlete, Workout w) => 100.0 * w.Laps.Sum(l => l.Duration * athlete.HeartRateReserveFraction(l.HR) * 0.64 * Math.Exp((athlete.Gender == Domain.Enums.Gender.MALE ? 1.92 : 1.67) * athlete.HeartRateReserveFraction(l.HR))) / (3600 * 0.64 * athlete.HeartRateReserveFraction(athlete.ThresholdHeartRate) * Math.Exp((athlete.Gender == Domain.Enums.Gender.MALE ? 1.92 : 1.67) * athlete.HeartRateReserveFraction(athlete.ThresholdHeartRate)));

}

