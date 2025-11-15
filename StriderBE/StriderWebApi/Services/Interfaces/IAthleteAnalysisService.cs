using System.Collections.Generic;
using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Services.Interfaces;

public interface IAthleteAnalysisService
{
    TrainingStatus Analyze(Athlete athlete, IEnumerable<TrainingSession> trainingSessions);
}

