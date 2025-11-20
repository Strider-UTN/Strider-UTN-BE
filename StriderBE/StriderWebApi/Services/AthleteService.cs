using StriderWebApi.Dto.Athlete;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Services.Interfaces;
using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Exceptions.Athlete;


namespace StriderWebApi.Services
{
    public class AthleteService(IAthleteRepository athleteRepository) : IAthleteService
    {

        private readonly IAthleteRepository _athleteRepository = athleteRepository;

        public async Task UpdateAthlete(Athlete athlete)
        {
            await _athleteRepository.UpdateAthleteAsync(athlete);
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