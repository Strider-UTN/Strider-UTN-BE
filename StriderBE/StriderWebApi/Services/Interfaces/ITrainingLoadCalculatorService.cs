using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Services.Interfaces;

public interface ITrainingLoadCalculatorService
{
    double ExponentialAverageTrainingLoad(Athlete athlete, int lookBackInDays, double weight);

}

