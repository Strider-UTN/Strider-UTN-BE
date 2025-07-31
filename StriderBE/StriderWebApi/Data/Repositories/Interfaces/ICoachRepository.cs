using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Data.Repositories.Interfaces
{
    public interface ICoachRepository
    {
        Task<Coach?> GetCoachByIdAsync(int id);
        Task<Coach?> GetCoachByUsernameAsync(string username);
        Task<Coach?> GetCoachByEmailAsync(string email);
        Task<List<Coach>> GetAllCoachesAsync();

        Task AddCoachAsync(Coach coach);
        Task UpdateCoachAsync(Coach coach);
        Task DeleteCoachAsync(Coach coach);
    }
}
