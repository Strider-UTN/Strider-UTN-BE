using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Data.Repositories.Interfaces
{
    public interface IAthleteRepository
    {
        Task<Athlete?> GetAthleteByIdAsync(int id);
        Task<Athlete?> GetAthleteByUsernameAsync(string username);
        Task<Athlete?> GetAthleteByEmailAsync(string email);
        Task<List<Athlete>> GetAllAthletesAsync();

        Task AddAthleteAsync(Athlete athlete);
        Task UpdateAthleteAsync(Athlete athlete);
        Task DeleteAthleteAsync(Athlete athlete);
    }
}
