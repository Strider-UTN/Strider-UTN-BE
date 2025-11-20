using System.Threading;
using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Services.Interfaces;

public interface IAthleteService
{
    Task UpdateAthlete(Athlete athlete);
    Task<bool> GetActiveStatusAsync(int athleteId, CancellationToken cancellationToken = default);
    Task UpdateActiveStatusAsync(int athleteId, bool isActive, CancellationToken cancellationToken = default);
}
