using StriderWebApi.Controllers;
using StriderWebApi.Data.Repositories;
using StriderWebApi.Dto.Athlete;
using StriderWebApi.Model;
using StriderWebApi.Services.Interfaces;


namespace StriderWebApi.Services
{
    public class AthleteService(AthleteRepository athleteRepository) : IAthleteService
    {

        private readonly AthleteRepository _athleteRepository = athleteRepository;

        public async Task<Athlete> GetAthleteById(int athleteId)
        {
            Domain.DomainClasses.Athlete athlete = await _athleteRepository.GetAthleteByIdAsync(athleteId) ?? throw new AthleteNotFoundException();
            return new Athlete(
                athlete.Id,
                athlete.Username,
                athlete.FullName,
                athlete.Email,
                athlete.Gender,
                athlete.Address,
                athlete.VO2Max,
                athlete.MedicalConditions,
                athlete.Objectives,
                athlete.BirthDate
            );
        }

        public async Task<AthleteFeedbackResponseDTO> GetAthleteFeedback(int athleteId)
        {

            Athlete athlete = await GetAthleteById(athleteId);
        
            List<Workout> workoutsPendingFeedback = athlete.WorkoutsPendingFeedback();
                    List<Workout> workoutsWithFeedback = athlete.WorkoutsWithFeedback();
                    List<Workout> workoutsThisWeek = athlete.WorkoutsThisWeek();
                    AthleteFeedbackResponseDTO responseDTO = new(
                        workoutsPendingFeedback.Count,
                        workoutsWithFeedback.Count,
                        workoutsThisWeek.Count,
                        [.. workoutsPendingFeedback.Select(w =>
                            {
                                if (w.LinkedSession == null) {
                                    throw new Exception("Workout does not have linked session");
                                }
                                Comparer comparer = new();
                                return new AthleteFeedbackResponseDTO.Workout(
                                w.Id,
                                w.Name,
                                w.Date,
                                w.Duration,
                                w.AverageHR,
                                w.Comments,
                                w.LinkedSession.IntervalCount(),
                                w.LinkedSession.ActiveIntervalCount(),
                                comparer.AverageCompletionPercentage(athlete,w,w.LinkedSession),
                                [.. w.LinkedSession.Intervals.Select((interval,index) => {
                                    Lap lap = w.GetLap(index);
                                    return new AthleteFeedbackResponseDTO.Workout.Interval(
                                        index,
                                        interval.IsActive(),
                                        new AthleteFeedbackResponseDTO.Workout.Interval.Planned(
                                            interval.Distance(athlete),
                                            interval.Duration(athlete),
                                            interval.Speed.Speed(athlete)
                                        ),
                                        new AthleteFeedbackResponseDTO.Workout.Interval.Actual(
                                            lap.Distance,
                                            lap.Duration,
                                            lap.Speed,
                                            lap.HR
                                        ),
                                        comparer.MatchPercentage(athlete,lap, interval)
                                    );
                                })]
                            );
                            }
                        )]
                    );
            return responseDTO;
        }

        public void UpdateAthlete(Athlete athlete)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}