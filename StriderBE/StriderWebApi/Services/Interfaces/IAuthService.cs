using Google.Apis.Auth;
using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> HandleGoogleLoginAsync(GoogleJsonWebSignature.Payload payload, UserTypeEnum userType);
        Task<string> HandleLoginAsync(string email, string password, UserTypeEnum userType);
    }
}
