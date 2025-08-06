using StriderWebApi.Dto.Athlete;
using StriderWebApi.Model;

namespace StriderWebApi.Services.Interfaces;

public interface IAthleteService
{
    Task<AthleteFeedbackResponseDTO> GetAthleteFeedback(int athleteId);
    Task UpdateAthlete(Athlete athlete);
}
