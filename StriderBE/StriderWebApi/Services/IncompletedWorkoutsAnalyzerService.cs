using System;
using System.Linq;
using System.Collections.Generic;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Domain.Enums;
using StriderWebApi.Dto.Athlete;
using StriderWebApi.Services.Interfaces;

namespace StriderWebApi.Services;

public class IncompletedWorkoutsAnalyzerService(int incompletedWorkoutsThreshold) : IAthleteAnalysisService
{
    private readonly int _incompletedWorkoutsThreshold = incompletedWorkoutsThreshold;

    public AthleteAnalysisResultResponseDto Analyze(Athlete athlete, IEnumerable<TrainingSession> trainingSessions)
    {
        var incompletedTrainingSessions = trainingSessions
            .Where(s => s.Date < DateTime.Now.AddDays(-1) &&
                        s.Date > DateTime.Now.AddDays(-7) &&
                        s.Athletes.Any(a => a.AthleteId == athlete.Id) &&
                        s.Athletes.First(a => a.AthleteId == athlete.Id).Status == SessionStatus.Pending)
            .OrderBy(s => s.Date)
            .ToList();

        int incompletedWorkouts = incompletedTrainingSessions.Count;

        if (incompletedWorkouts > _incompletedWorkoutsThreshold)
        {
            string trainingSessionDetails = string.Join("\n", incompletedTrainingSessions.Select(s =>
                $"   • {s.Date:MMM dd, yyyy} - {s.Name ?? "Unnamed Session"}"));

            return new AthleteAnalysisResultResponseDto
            {
                Title = $"Athlete {athlete.FullName} has {incompletedWorkouts} incomplete workouts during the last week",
                Description = $"The following workouts were not completed:\n\n{trainingSessionDetails}\n\n",
                Type = AthleteAnalysisResultType.Warning
            };
        }

        return new AthleteAnalysisResultResponseDto
        {
            Title = $"Athlete {athlete.FullName} has completed all workouts during the last week",
            Description = $"Good job! The athlete has completed all workouts during the last week.",
            Type = AthleteAnalysisResultType.Ok
        };

    }

    
}

