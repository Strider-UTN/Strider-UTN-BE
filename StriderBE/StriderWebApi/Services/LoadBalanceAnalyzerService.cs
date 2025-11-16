using System.Collections.Generic;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Dto.Athlete;
using StriderWebApi.Services.Interfaces;

namespace StriderWebApi.Services;

public class LoadBalanceAnalyzerService(
	ITrainingLoadCalculatorService calculator,
	int acuteLookBackInDays,
	int chronicLookBackInDays,
	double weightFactor,
	double undertrainmentThreshold,
	double overreachThreshold,
	double overTrainingThreshold) : IAthleteAnalysisService
{
	private readonly ITrainingLoadCalculatorService _calculator = calculator;
	private readonly int _acuteLookBackInDays = acuteLookBackInDays;
	private readonly int _chronicLookBackInDays = chronicLookBackInDays;
	private readonly double _weightFactor = weightFactor;
	private readonly double _undertrainmentThreshold = undertrainmentThreshold;
	private readonly double _overreachThreshold = overreachThreshold;
	private readonly double _overTrainingThreshold = overTrainingThreshold;

	public AthleteAnalysisResultResponseDto Analyze(Athlete athlete, IEnumerable<TrainingSession> trainingSessions)
	{
		double chronicTrainingLoad = _calculator.ExponentialAverageTrainingLoad(athlete, _chronicLookBackInDays, _weightFactor);
		double acuteTrainingLoad = _calculator.ExponentialAverageTrainingLoad(athlete, _acuteLookBackInDays, _weightFactor);
		double acRatio = acuteTrainingLoad / (chronicTrainingLoad + 0.0000001);

		if (acRatio < _undertrainmentThreshold)
		{
			return new AthleteAnalysisResultResponseDto()
			{
				Title = $"Athlete {athlete.FullName} may be undertrained",
				Description = $"The athlete's acute-chronic ratio is {acRatio}. Set minimum limit for undertraining was {_undertrainmentThreshold}. Athlete may be undertraining.",
				Type = AthleteAnalysisResultType.Warning
			};
		}

		if (acRatio > _overreachThreshold && acRatio < _overTrainingThreshold)
		{
			return new AthleteAnalysisResultResponseDto()
			{
				Title = $"Athlete {athlete.FullName} may be overreaching",
				Description = $"The athlete's acute-chronic ratio is {acRatio}. Set limits for overreach were {_overreachThreshold} and {_overTrainingThreshold}. Athlete may be overreaching.",
				Type = AthleteAnalysisResultType.Warning
			};
		}

		if (acRatio > _overTrainingThreshold)
		{
			return new AthleteAnalysisResultResponseDto()
			{
				Title = $"Athlete {athlete.FullName} may be overtrained",
				Description = $"The athlete's acute-chronic ratio is {acRatio}. Set limit for overtraining was {_overTrainingThreshold}. Athlete may be overtraining.",
				Type = AthleteAnalysisResultType.Warning
			};
		}

		return new AthleteAnalysisResultResponseDto()
		{
			Title = $"Athlete {athlete.FullName} is in a good load balance",
			Description = $"The athlete's acute-chronic ratio is {acRatio}. Set limits for overreach were {_overreachThreshold} and {_overTrainingThreshold}. Athlete may be overreaching.",
			Type = AthleteAnalysisResultType.Ok
		};
	}

}

