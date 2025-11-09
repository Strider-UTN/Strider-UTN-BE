using StriderWebApi.Controllers;
using StriderWebApi.Dto.Athlete;
using StriderWebApi.Model;
using StriderWebApi.Services.Interfaces;
using StriderWebApi.Data.Repositories.Interfaces;


namespace StriderWebApi.Services
{
    public class AthleteService(IAthleteRepository athleteRepository) : IAthleteService
    {

        private readonly IAthleteRepository _athleteRepository = athleteRepository;

        public async Task<Athlete> GetAthleteByIdAsync(int athleteId)
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
            throw new NotImplementedException(); // TODO
        }

        public async Task UpdateAthlete(Athlete athlete)
        {
            Domain.DomainClasses.Athlete domainAthlete = new()
            {
                Id = athlete.Id,
                Username = athlete.Username,
                FullName = athlete.Name,
                PhoneNumber = athlete.PhoneNumber ?? "" ,
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

        public async Task<bool> GetActiveStatusAsync(int athleteId, CancellationToken cancellationToken = default)
        {
            var status = await _athleteRepository.GetActiveStatusAsync(athleteId, cancellationToken);
            if (!status.HasValue)
            {
                throw new AthleteNotFoundException();
            }

            return status.Value;
        }

        public async Task UpdateActiveStatusAsync(int athleteId, bool isActive, CancellationToken cancellationToken = default)
        {
            var updated = await _athleteRepository.UpdateActiveStatusAsync(athleteId, isActive, cancellationToken);
            if (!updated)
            {
                throw new AthleteNotFoundException();
            }
        }
    }
}