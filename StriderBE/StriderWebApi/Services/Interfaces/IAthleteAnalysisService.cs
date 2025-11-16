using System.Collections.Generic;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Dto.Athlete;

namespace StriderWebApi.Services.Interfaces;

public interface IAthleteAnalysisService
{
    AthleteAnalysisResultResponseDto Analyze(Athlete athlete, IEnumerable<TrainingSession> trainingSessions);
}

