using System;
using System.Linq;
using System.Collections.Generic;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Domain.Enums;
using StriderWebApi.Dto.Athlete;
using StriderWebApi.Services.Interfaces;

namespace StriderWebApi.Services;

public class StressBalanceAnalyzerService(
	ITrainingLoadCalculatorService calculator,
	int acuteLookBackInDays,
	int chronicLookBackInDays,
	double stressBalanceThreshold,
	int competitionLookForwardInDays,
	double weightFactor) : IAthleteAnalysisService
{
	private readonly ITrainingLoadCalculatorService _calculator = calculator;
	private readonly int _acuteLookBackInDays = acuteLookBackInDays;
	private readonly int _chronicLookBackInDays = chronicLookBackInDays;
	private readonly double _stressBalanceThreshold = stressBalanceThreshold;
	private readonly int _competitionLookForwardInDays = competitionLookForwardInDays;
	private readonly double _weightFactor = weightFactor;

	public AthleteAnalysisResultResponseDto Analyze(Athlete athlete, IEnumerable<TrainingSession> trainingSessions)
	{
		var nextCompetition = trainingSessions
			.Where(s =>
				s.Date > DateTime.Now &&
				s.Date < DateTime.Now.AddDays(_competitionLookForwardInDays) &&
				s.Athletes.Any(a => a.AthleteId == athlete.Id) &&
				(s.Category == TrainingCategory.PrepCompetition || s.Category == TrainingCategory.MainCompetition)
			)
			.OrderBy(s => s.Date)
			.FirstOrDefault();
		
		if (nextCompetition == null)
		{
			return new AthleteAnalysisResultResponseDto()
			{
				Title = $"Datos insuficientes para el análisis de balance de estrés",
				Description = $"No se puede analizar el balance de estrés para el atleta {athlete.FullName}. No hay competencias próximas programadas dentro de los próximos {_competitionLookForwardInDays} días.",
				Type = AthleteAnalysisResultType.NoData
			};
		}
		
		DateTime nextCompetitionDate = nextCompetition.Date;
		double stressBalance =  CalculateTrainingStressBalance(athlete, trainingSessions);

		if (stressBalance > _stressBalanceThreshold)
		{
			return new AthleteAnalysisResultResponseDto()
			{
				Title = $"El atleta {athlete.FullName} puede estar sobrecargado para las competencias",
				Description = $"El balance de estrés del atleta es {stressBalance:F2}. El límite establecido fue {_stressBalanceThreshold:F2} para competencias. El atleta puede estar sobrecargado para la competencia en los próximos {nextCompetitionDate.Subtract(DateTime.Now).Days} días.",
				Type = AthleteAnalysisResultType.Warning
			};
		}

		return new AthleteAnalysisResultResponseDto()
		{
			Title = $"No se detectaron problemas de balance de estrés",
			Description = $"No se puede proporcionar un análisis de balance de estrés para el atleta {athlete.FullName}. El balance de estrés del atleta ({stressBalance:F2}) está dentro de los límites aceptables (umbral: {_stressBalanceThreshold:F2}).",
			Type = AthleteAnalysisResultType.Ok
		};
	}

	private double CalculateTrainingStressBalance(Athlete athlete, IEnumerable<TrainingSession> trainingSessions)
	{
		return _calculator.ExponentialAverageTrainingLoad(athlete, trainingSessions, _acuteLookBackInDays, _weightFactor) - _calculator.ExponentialAverageTrainingLoad(athlete, trainingSessions, _chronicLookBackInDays, _weightFactor);
	}

}

