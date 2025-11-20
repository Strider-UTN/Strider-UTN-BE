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

        if (trainingSessions.Count() == 0)
        {
            return new AthleteAnalysisResultResponseDto
            {
                Title = $"No se encontraron sesiones de entrenamiento",
                Description = $"No se puede proporcionar un análisis de entrenamientos incompletos para el atleta {athlete.FullName}. No hay sesiones de entrenamiento para analizar.",
                Type = AthleteAnalysisResultType.NoData
            };
        }

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
                $"   • {s.Date:MMM dd, yyyy} - {s.Name ?? "Sesión sin nombre"}"));

            return new AthleteAnalysisResultResponseDto
            {
                Title = $"El atleta {athlete.FullName} tiene {incompletedWorkouts} entrenamientos incompletos durante la última semana",
                Description = $"Los siguientes entrenamientos no fueron completados:\n\n{trainingSessionDetails}\n\n",
                Type = AthleteAnalysisResultType.Warning
            };
        }

        return new AthleteAnalysisResultResponseDto
        {
            Title = $"El atleta {athlete.FullName} ha completado todos los entrenamientos durante la última semana",
            Description = $"¡Buen trabajo! El atleta ha completado todos los entrenamientos durante la última semana.",
            Type = AthleteAnalysisResultType.Ok
        };

    }

    
}

