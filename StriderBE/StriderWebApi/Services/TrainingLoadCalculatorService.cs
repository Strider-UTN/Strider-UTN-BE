using System;
using System.Linq;
using StriderWebApi.Domain.Enums;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Services.Interfaces;

namespace StriderWebApi.Services;

public class TrainingLoadCalculatorService : ITrainingLoadCalculatorService
{
    private const double THRESHOLD_HR_FACTOR = 0.64;
    private const double REFERENCE_DURATION = 3600;

    public double TrainingLoadFrom(Athlete athlete, CompletedWorkout w) => 100.0 * w.Laps.Sum(l => LapTrainingLoad(athlete, l.Duration)) / LapTrainingLoad(athlete, REFERENCE_DURATION);
    public double ExponentialAverageTrainingLoad(Athlete athlete, IEnumerable<TrainingSession> trainingSessions,  int lookBackInDays, double weightFactor)
    {
        var completedWorkouts = trainingSessions.SelectMany(ts => ts.Athletes)
                                                .Where(ath => ath.AthleteId == athlete.Id && ath.CompletedWorkouts is not null && ath.CompletedWorkouts.Count > 0)
                                                .Select(ath => ath.CompletedWorkouts.First());

        List<CompletedWorkout> workouts = completedWorkouts.Where(w => w.Date >= DateTime.Now.AddDays(-lookBackInDays)).ToList();
        double totalTrainingLoad = 0;
        for (int i = 0; i < workouts.Count; i++){
            double daysDifference = (DateTime.Now - workouts[i].Date).TotalDays;
            double weight = Math.Pow(weightFactor, daysDifference);
            totalTrainingLoad += TrainingLoadFrom(athlete, workouts[i]) * weight;
        }
        return totalTrainingLoad;
    }

    private double LapTrainingLoad(Athlete athlete, double duration) => duration * THRESHOLD_HR_FACTOR * Math.Exp(GenderFactor(athlete.Gender) * HeartRateReserveFraction(athlete, athlete.ThresholdHeartRate));
    private double HeartRateReserveFraction(Athlete athlete, double hr) => (hr - athlete.RestingHeartRate) / (athlete.MaximumHeartRate - athlete.RestingHeartRate);
    private double GenderFactor(Gender gender) => gender == Gender.MALE ? 1.92 : 1.67;


}

