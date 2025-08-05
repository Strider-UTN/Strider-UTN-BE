using StriderWebApi.Data.Repositories;
using StriderWebApi.Dto.Coach;
using StriderWebApi.Exceptions.Coach;
using StriderWebApi.Model;
using StriderWebApi.Services.Interfaces;
using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Services;

public class CoachService(CoachRepository coachRepository, AthleteService athleteService) : ICoachService
{

    private readonly CoachRepository _coachRepository = coachRepository;
    private readonly AthleteService _athleteService = athleteService;

    public async Task<Coach> GetCoachById(int id)
    {
        Domain.DomainClasses.Coach coach = await _coachRepository.GetCoachByIdAsync(id) ?? throw new CoachNotFoundException();
        return new Coach
        {
            Id = coach.Id,
            Username = coach.Username,
            Name = coach.FullName,
            Email = coach.Email,
            Gender = (Gender)coach.Gender,
            Address = coach.Address,
            BirthDate = coach.BirthDate
        };
    }

    public async Task<CoachResponseDTO> GetCoachIndividualAthletes(int coachId)
    {
        Coach coach = await GetCoachById(coachId);
        return new CoachResponseDTO
        {
            Name = coach.Name,
            TotalAthletes = coach.TotalIndividualAthletes(),
            ActiveAthletes = coach.ActiveIndividualAthletes(),
            InactiveAthletes = coach.InactiveIndividualAthletes(),
            WorkoutsCompleted = coach.TotalWorkoutsCompletedByIndividualAthletes(),
            Athletes = coach.Athletes.Select(a => new CoachResponseDTO.Athlete
            {
                Id = a.Id,
                Name = a.Name,
                Age = DateTime.Today.Subtract(a.BirthDate).Days / 365,
                Objectives = a.Objectives,
                TotalWorkouts = a.TotalWorkoutsCompleted(),
                LastWorkoutDate = a.GetLastWorkoutDate(),
                IsActive = a.IsActive(),
                Ailments = a.GetActiveAilments().Select(ailment => new CoachResponseDTO.Athlete.Ailment
                {
                    Name = ailment.GetName(),
                    Treatment = ailment.Treatment
                }).ToList()
            }).ToList()
        };
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