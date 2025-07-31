namespace StriderWebApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> HandleLogin(string? username, string? password);
    }
}
