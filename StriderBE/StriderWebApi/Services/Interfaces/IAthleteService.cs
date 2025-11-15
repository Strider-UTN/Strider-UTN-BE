using System.Threading;
using StriderWebApi.Dto.Athlete;
using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Services.Interfaces;

public interface IAthleteService
{
    Task<AthleteFeedbackResponseDTO> GetAthleteFeedback(int athleteId);
    Task UpdateAthlete(Athlete athlete);
    Task<Athlete> GetAthleteByIdAsync(int athleteId);
    Task<bool> GetActiveStatusAsync(int athleteId, CancellationToken cancellationToken = default);
    Task UpdateActiveStatusAsync(int athleteId, bool isActive, CancellationToken cancellationToken = default);
}
