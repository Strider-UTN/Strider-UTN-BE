using StriderWebApi.Controllers;
using StriderWebApi.Data.Repositories;
using StriderWebApi.Dto.Athlete;
using StriderWebApi.Model;
using StriderWebApi.Services.Interfaces;
using StriderWebApi.Domain.Enums;


namespace StriderWebApi.Services
{
    public class AthleteService(AthleteRepository athleteRepository) : IAthleteService
    {

        private readonly AthleteRepository _athleteRepository = athleteRepository;

        public async Task<Athlete> GetAthleteById(int athleteId)
        {
            Domain.DomainClasses.Athlete athlete = await _athleteRepository.GetAthleteByIdAsync(athleteId) ?? throw new AthleteNotFoundException();
            return new Athlete
            {
                Id = athlete.Id,
                PhoneNumber = athlete.PhoneNumber,
                CreatedBy = athlete.CreatedBy,
                Username = athlete.Username,
                Name = athlete.FullName,
                Email = athlete.Email,
                Gender = athlete.Gender,
                Address = athlete.Address,
                VO2Max = athlete.VO2Max,
                MedicalConditions = athlete.MedicalConditions,
                Objectives = athlete.Objectives,
                BirthDate = athlete.BirthDate,
                EmergencyContactName = athlete.EmergencyContactName,
                EmergencyContactPhone = athlete.EmergencyContactPhone,
                EmergencyContactRelationship = athlete.EmergencyContactRelationship
                
            };
        }

        public async Task<AthleteFeedbackResponseDTO> GetAthleteFeedback(int athleteId)
        {

            Athlete athlete = await GetAthleteById(athleteId);
        
            List<Workout> workoutsPendingFeedback = athlete.WorkoutsPendingFeedback();
                    List<Workout> workoutsWithFeedback = athlete.WorkoutsWithFeedback();
                    List<Workout> workoutsThisWeek = athlete.WorkoutsThisWeek();
                    AthleteFeedbackResponseDTO responseDTO = new()
                    {
                        WorkoutsPendingFeedback = workoutsPendingFeedback.Count,
                        WorkoutsWithFeedback = workoutsWithFeedback.Count,
                        WorkoutsThisWeek = workoutsThisWeek.Count,
                        Workouts = workoutsPendingFeedback.Select(w =>
                            {
                                if (w.Session == null) {
                                    throw new Exception("Workout does not have linked session");
                                }
                                WorkoutComparer comparer = new();
                                return new AthleteFeedbackResponseDTO.Workout
                                {
                                    Id = w.Id,
                                    Name = w.Name,
                                    Date = w.Date,
                                    Duration = w.Duration,
                                    AverageHR = w.AverageHR,
                                    Comments = w.Comments ?? string.Empty,
                                    Count = w.Session.IntervalCount(),
                                    ActiveIntervalCount = w.Session.ActiveIntervalCount(),
                                    Value = comparer.AverageCompletionPercentage(athlete, w, w.Session),
                                    Intervals = w.Session.Intervals.Select((interval, index) => {
                                        Lap lap = w.GetLap(index);
                                        return new AthleteFeedbackResponseDTO.Workout.Interval
                                        {
                                            Index = index,
                                            IsActive = interval.IsActive,
                                            PlannedInterval = new AthleteFeedbackResponseDTO.Workout.Interval.Planned
                                            {
                                                Distance = interval.GetDistance(athlete),
                                                Duration = interval.GetDuration(athlete),
                                                Speed = interval.SpeedType.GetSpeed(athlete, interval.Speed, interval.Percentage)
                                            },
                                            ActualInterval = new AthleteFeedbackResponseDTO.Workout.Interval.Actual
                                            {
                                                Distance = lap.Distance,
                                                Duration = lap.Duration,
                                                Speed = lap.Speed,
                                                HR = lap.HR
                                            },
                                            MatchPercentage = comparer.MatchPercentage(athlete, lap, interval)
                                        };
                                    }).ToList()
                                };
                            }
                        ).ToList()
                    };
            return responseDTO;
        }

        public async Task UpdateAthlete(Athlete athlete)
        {
            Domain.DomainClasses.Athlete domainAthlete = new()
            {
                Id = athlete.Id,
                Username = athlete.Username,
                FullName = athlete.Name,
                PhoneNumber = athlete.PhoneNumber,
                Email = athlete.Email,
                Gender = athlete.Gender,
                Address = athlete.Address,
                VO2Max = athlete.VO2Max,
                MedicalConditions = athlete.MedicalConditions,
                Objectives = athlete.Objectives,
                BirthDate = athlete.BirthDate,
                Workouts = athlete.Workouts.Select(w => new Domain.DomainClasses.Workout
                {
                    Id = w.Id,
                    Name = w.Name,
                    Distance = w.Distance,
                    Date = w.Date,
                    Duration = w.Duration,
                    AverageHR = w.AverageHR,
                    State = w.State,
                    Type = w.Type,
                    Comments = w.Comments,
                    CoachFeedback = w.CoachFeedback,
                    IsReviewed = w.IsReviewed,
                    AthleteId = w.Athlete.Id,
                    SessionId = w.Session?.Id,
                    Laps = w.Laps.Select(l => new Domain.DomainClasses.Lap
                    {
                        Id = l.Id,
                        Index = l.Index,
                        Distance = l.Distance,
                        Duration = l.Duration,
                        Speed = l.Speed,
                        HR = l.HR,
                        StartTime = l.StartTime,
                        CoachFeedback = l.CoachFeedback,
                        WorkoutId = w.Id
                    }).ToList()
                }).ToList()
            };
            
            await _athleteRepository.UpdateAthleteAsync(domainAthlete);
        }
    }
}