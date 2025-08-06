using StriderWebApi.Dto.Coach;

namespace StriderWebApi.Services.Interfaces;


public interface ICoachService
{
    Task<CoachResponseDTO> GetCoachIndividualAthletes(int coachId);
    Task PostWorkoutFeedbackAsync(int athleteId, int workoutId, CoachFeedbackDTO feedback);
}


