using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Services.Interfaces;

public interface ITrainingLoadCalculatorService
{
    double ExponentialAverageTrainingLoad(Athlete athlete, IEnumerable<TrainingSession> trainingSessions, int lookBackInDays, double weight);

}

