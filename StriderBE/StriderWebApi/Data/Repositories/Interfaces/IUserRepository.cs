using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Data.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEmailAsync(string email, UserTypeEnum? userType = null);
        Task<bool> UserExistsByEmailAsync(string email, UserTypeEnum userType);
        Task<bool> UpdateUserAsync(User user);
        Task<IReadOnlyList<User>> GetAllUsersAsync();
        Task<User?> GetUserByPasswordResetTokenAsync(string token);
    }
}
