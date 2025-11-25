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
		double chronicTrainingLoad = _calculator.ExponentialAverageTrainingLoad(athlete, trainingSessions, _chronicLookBackInDays, _weightFactor);
		double acuteTrainingLoad = _calculator.ExponentialAverageTrainingLoad(athlete, trainingSessions, _acuteLookBackInDays, _weightFactor);
		
		if (chronicTrainingLoad == 0 && acuteTrainingLoad == 0)
		{
			return new AthleteAnalysisResultResponseDto()
			{
				Title = $"Datos insuficientes para el análisis de balance de carga",
				Description = $"No se puede analizar el balance de carga para el atleta {athlete.FullName}. No hay datos de carga de entrenamiento disponibles para los períodos requeridos.",
				Type = AthleteAnalysisResultType.NoData
			};
		}
		
		double acRatio = acuteTrainingLoad / (chronicTrainingLoad + 0.0000001);

		if (acRatio < _undertrainmentThreshold)
		{
			return new AthleteAnalysisResultResponseDto()
			{
				Title = $"El atleta {athlete.FullName} puede estar subentrenado",
				Description = $"La relación aguda-crónica del atleta es {acRatio:F2}. El límite mínimo establecido para subentrenamiento fue {_undertrainmentThreshold:F2}. El atleta puede estar subentrenando.",
				Type = AthleteAnalysisResultType.Warning
			};
		}

		if (acRatio > _overreachThreshold && acRatio < _overTrainingThreshold)
		{
			return new AthleteAnalysisResultResponseDto()
			{
				Title = $"El atleta {athlete.FullName} puede estar sobrecargándose",
				Description = $"La relación aguda-crónica del atleta es {acRatio:F2}. Los límites establecidos para sobrecarga fueron {_overreachThreshold:F2} y {_overTrainingThreshold:F2}. El atleta puede estar sobrecargándose.",
				Type = AthleteAnalysisResultType.Warning
			};
		}

		if (acRatio > _overTrainingThreshold)
		{
			return new AthleteAnalysisResultResponseDto()
			{
				Title = $"El atleta {athlete.FullName} puede estar sobreentrenado",
				Description = $"La relación aguda-crónica del atleta es {acRatio:F2}. El límite establecido para sobreentrenamiento fue {_overTrainingThreshold:F2}. El atleta puede estar sobreentrenando.",
				Type = AthleteAnalysisResultType.Warning
			};
		}

		return new AthleteAnalysisResultResponseDto()
		{
			Title = $"El atleta {athlete.FullName} tiene un buen balance de carga",
			Description = $"La relación aguda-crónica del atleta es {acRatio:F2}. Los límites establecidos para sobrecarga fueron {_undertrainmentThreshold:F2} y {_overreachThreshold:F2}. El atleta tiene un balance de carga adecuado.",
			Type = AthleteAnalysisResultType.Ok
		};
	}

}

