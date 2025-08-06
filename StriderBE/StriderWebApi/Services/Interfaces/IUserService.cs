using StriderWebApi.Dto.UserCreation;

namespace StriderWebApi.Services.Interfaces
{
    public interface IUserService
    {
        Task CreateCoachAsync(CreateCoachDto dto);
        Task CreateAthleteAsync(CreateAthleteDto dto);
        Task ActivateAccountAsync(ActivateAccountDto dto);
    }
}
