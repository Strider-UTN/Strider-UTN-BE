using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Dto.Coach;
using StriderWebApi.Exceptions.Coach;
using StriderWebApi.Model;
using StriderWebApi.Services.Interfaces;

namespace StriderWebApi.Services;

public class CoachService(ICoachRepository coachRepository, IAthleteService athleteService) : ICoachService
{

    private readonly ICoachRepository _coachRepository = coachRepository;
    private readonly IAthleteService _athleteService = athleteService;

    public async Task<Coach> GetCoachById(int id)
    {
        Domain.DomainClasses.Coach coach = await _coachRepository.GetCoachByIdAsync(id) ?? throw new CoachNotFoundException();
        return new Coach
        {
            Id = coach.Id,
            CreatedBy = coach.CreatedBy,
            PhoneNumber = coach.PhoneNumber,
            Username = coach.Username,
            Name = coach.FullName,
            Email = coach.Email,
            Gender = coach.Gender,
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
                Email = a.Email,
                PhoneNumber = a.PhoneNumber,
                Experience = a.YearsOfExperience(),
                WeeklyDistance = a.WeeklyDistance(),
                Age = a.Age(),
                BirthYear = a.BirthDate.Year,
                Height = a.Height,
                Weight = a.Weight,
                MonthlyDistance = a.MonthlyDistance(),
                EmergencyContactName = a.EmergencyContactName,
                EmergencyContactPhone = a.EmergencyContactPhone,
                EmergencyContactRelationship = a.EmergencyContactRelationship,
                RegistrationDate = a.CreatedDate,
                LastActivityDate = a.GetLastWorkoutDate()

            }).ToList()
        };
    }

    public async Task PostWorkoutFeedbackAsync(int athleteId, int workoutId, CoachFeedbackDTO feedback)
    {
        Athlete athlete = await _athleteService.GetAthleteByIdAsync(athleteId);
        Workout workout = athlete.GetWorkoutById(workoutId);
        workout.CoachFeedback = feedback.Feedback;
        foreach (int index in feedback.LapFeedbacks.Keys) {
            Lap lap = workout.GetLap(index);
            lap.CoachFeedback = feedback.LapFeedbacks[index];
        }
        await _athleteService.UpdateAthlete(athlete);
    }
}