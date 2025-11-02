using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Data.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEmailAsync(string email, UserTypeEnum? userType = null);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<bool> UserExistsByEmailAsync(string email, UserTypeEnum userType);
        Task<bool> UserExistsByUsernameAsync(string username, UserTypeEnum userType);
        Task<bool> UpdateUserAsync(User user);
    }
}
