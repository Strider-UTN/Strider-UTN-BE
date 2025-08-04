using StriderWebApi.Dto.Athlete;

namespace StriderWebApi.Services.Interfaces;

public interface IAthleteService
{
    Task<AthleteFeedbackResponseDTO> GetAthleteFeedback(int athleteId);
}
