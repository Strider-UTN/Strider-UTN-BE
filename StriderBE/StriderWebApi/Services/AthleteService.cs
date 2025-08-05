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
                Username = athlete.Username,
                Name = athlete.FullName,
                Email = athlete.Email,
                Gender = (Gender)athlete.Gender,
                Address = athlete.Address,
                VO2Max = athlete.VO2Max,
                MedicalConditions = athlete.MedicalConditions,
                Objectives = athlete.Objectives,
                BirthDate = athlete.BirthDate
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
                                Comparer comparer = new();
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

        public void UpdateAthlete(Athlete athlete)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}