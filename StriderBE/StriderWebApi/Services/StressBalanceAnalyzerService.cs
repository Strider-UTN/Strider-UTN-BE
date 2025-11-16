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
		DateTime nextCompetition = trainingSessions
			.Where(s =>
				s.Date > DateTime.Now &&
				s.Date < DateTime.Now.AddDays(_competitionLookForwardInDays) &&
				s.Athletes.Any(a => a.AthleteId == athlete.Id) &&
				(s.Category == TrainingCategory.PrepCompetition || s.Category == TrainingCategory.MainCompetition)
			)
			.OrderBy(s => s.Date)
			.First()
			.Date;
		double stressBalance =  CalculateTrainingStressBalance(athlete);

		if (stressBalance < _stressBalanceThreshold)
		{
			return new AthleteAnalysisResultResponseDto()
			{
				Title = $"Athlete {athlete.FullName} may be overloaded for competitions",
				Description = $"The athlete's stress balance is {stressBalance}. Set limit was {_stressBalanceThreshold} for competitions. Athlete may be overloaded for competition in the next {nextCompetition.Subtract(DateTime.Now).Days} days.",
				Type = AthleteAnalysisResultType.Warning
			};
		}

		return new AthleteAnalysisResultResponseDto()
		{
			Title = $"Athlete {athlete.FullName} is not overloaded for competitions",
			Description = $"The athlete's stress balance is {stressBalance}. Set limit was {_stressBalanceThreshold} for competitions. Athlete is not overloaded for competitions.",
			Type = AthleteAnalysisResultType.Ok
		};
	}

	private double CalculateTrainingStressBalance(Athlete athlete)
	{
		return _calculator.ExponentialAverageTrainingLoad(athlete, _acuteLookBackInDays, _weightFactor) - _calculator.ExponentialAverageTrainingLoad(athlete, _chronicLookBackInDays, _weightFactor);
	}

}

