using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.Enums;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Dto.Coach;
using StriderWebApi.Dto.Injuries;
using StriderWebApi.Exceptions.Coach;
using StriderWebApi.Services.Interfaces;
using System.Linq;

namespace StriderWebApi.Services;

public class CoachService(
    ICoachRepository coachRepository,
    IAthleteService athleteService,
    IAthleteInjuryRepository athleteInjuryRepository) : ICoachService
{

    private readonly ICoachRepository _coachRepository = coachRepository;
    private readonly IAthleteService _athleteService = athleteService;
    private readonly IAthleteInjuryRepository _athleteInjuryRepository = athleteInjuryRepository;

    public async Task<CoachResponseDTO> GetCoachIndividualAthletes(int coachId)
    {
        Coach coach = await _coachRepository.GetCoachByIdAsync(coachId) ?? throw new CoachNotFoundException();
        
        // Calculate workouts completed (placeholder - needs proper implementation)
        int workoutsCompleted = 0; // TODO: Implement TotalWorkoutsCompletedByIndividualAthletes
        
        var response = new CoachResponseDTO
        {
            Name = coach.FullName,
            WorkoutsCompleted = workoutsCompleted,
            Athletes = coach.Athletes.Select(a => new CoachResponseDTO.Athlete
            {
                Id = a.Id,
                Name = a.FullName,
                Email = a.Email,
                PhoneNumber = a.PhoneNumber,
                Experience = a.YearsOfExperience,
                WeeklyDistance = 0, // TODO: Implement WeeklyDistance calculation
                Age = DateTime.Now.Year - a.BirthDate.Year - (DateTime.Now.DayOfYear < a.BirthDate.DayOfYear ? 1 : 0),
                BirthYear = a.BirthDate.Year,
                Height = a.Height,
                Weight = a.Weight,
                MonthlyDistance = 0, // TODO: Implement MonthlyDistance calculation
                EmergencyContactName = a.EmergencyContactName,
                EmergencyContactPhone = a.EmergencyContactPhone,
                EmergencyContactRelationship = a.EmergencyContactRelationship,
                RegistrationDate = a.CreatedDate,
                LastActivityDate = a.Workouts.Any() ? a.Workouts.Max(w => w.Date) : a.CreatedDate

            }).ToList()
        };

        foreach (var athlete in response.Athletes)
        {
            var activeInjuries = await _athleteInjuryRepository.GetByAthleteIdAndStatusAsync(athlete.Id, InjuryStatus.Active);
            var summaries = activeInjuries.Select(MapToInjurySummary).ToList();
            athlete.HasActiveInjury = summaries.Count > 0;
            athlete.ActiveInjuries = summaries;
        }

        response.TotalAthletes = response.Athletes.Count;
        response.ActiveAthletes = response.Athletes.Count(a => !a.HasActiveInjury);
        response.InactiveAthletes = response.TotalAthletes - response.ActiveAthletes;

        return response;
    }

    public async Task PostWorkoutFeedbackAsync(int athleteId, int workoutId, CoachFeedbackDTO feedback)
    {
        Athlete athlete = await _athleteService.GetAthleteByIdAsync(athleteId);
        Workout? workout = athlete.Workouts.FirstOrDefault(w => w.Id == workoutId);
        if (workout == null)
        {
            throw new Exception($"Workout with id {workoutId} not found for athlete {athleteId}");
        }
        workout.CoachFeedback = feedback.Feedback;
        foreach (int index in feedback.LapFeedbacks.Keys) {
            Lap? lap = workout.Laps.FirstOrDefault(l => l.Index == index);
            if (lap != null)
            {
                lap.CoachFeedback = feedback.LapFeedbacks[index];
            }
        }
        await _athleteService.UpdateAthlete(athlete);
    }

    private static AthleteInjurySummaryDto MapToInjurySummary(Domain.DomainClasses.AthleteInjury injury)
    {
        return new AthleteInjurySummaryDto
        {
            Id = injury.Id,
            Title = injury.Title,
            Severity = injury.Severity,
            Status = injury.Status,
            AffectedArea = injury.AffectedArea,
            DiagnosisDate = injury.DiagnosisDate,
            RecoveryEstimateDate = injury.RecoveryEstimateDate,
            RecoveryDate = injury.RecoveryDate,
            Notes = injury.Notes
        };
    }
}