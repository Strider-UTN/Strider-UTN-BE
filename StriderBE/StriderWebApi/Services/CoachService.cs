using StriderWebApi.Data.Repositories;
using StriderWebApi.Dto.Coach;
using StriderWebApi.Exceptions.Coach;
using StriderWebApi.Model;
using StriderWebApi.Services.Interfaces;

namespace StriderWebApi.Services;

public class CoachService(CoachRepository coachRepository, AthleteService athleteService) : ICoachService
{

    private readonly CoachRepository _coachRepository = coachRepository;
    private readonly AthleteService _athleteService = athleteService;

    public async Task<Coach> GetCoachById(int id)
    {
        Domain.DomainClasses.Coach coach = await _coachRepository.GetCoachByIdAsync(id) ?? throw new CoachNotFoundException();
        return new Coach(
            coach.Id,
            coach.Username,
            coach.FullName,
            coach.Email,
            coach.Gender,
            coach.Address,
            coach.BirthDate
        );
    }

    public async Task<CoachResponseDTO> GetCoachIndividualAthletes(int coachId)
    {
        Coach coach = await GetCoachById(coachId);
        return new(
                    coach.Name,
                    coach.TotalIndividualAthletes(),
                    coach.ActiveIndividualAthletes(),
                    coach.InactiveIndividualAthletes(),
                    coach.TotalWorkoutsCompletedByIndividualAthletes(),
                    [.. coach.Athletes.Select(a => new CoachResponseDTO.Athlete(
                    a.Id,
                    a.Name,
                    DateTime.Today.Subtract(a.BirthDate).Days / 365,
                    a.Objectives,
                    a.TotalWorkoutsCompleted(),
                    a.GetLastWorkoutDate(),
                    a.IsActive(),
                    [.. a.GetActiveAilments().Select(a => new CoachResponseDTO.Athlete.Ailment(a.GetName(), a.Treatment))]
                    ))]
                );
    }

    public Task PostWorkoutFeedbackAsync(int athleteId, int workoutId, CoachFeedbackDTO feedback)
    {
        Athlete athlete = _athleteService.GetAthleteById(athleteId).Result;
        Workout workout = athlete.GetWorkoutById(workoutId);
        workout.CoachFeedback = feedback.Feedback;
        foreach (int index in feedback.LapFeedbacks.Keys) {
            Lap lap = workout.GetLap(index);
            lap.CoachFeedback = feedback.LapFeedbacks[index];
        }
        _athleteService.UpdateAthlete(athlete);
        return Task.CompletedTask;
    }
}