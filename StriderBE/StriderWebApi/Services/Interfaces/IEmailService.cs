namespace StriderWebApi.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendAccountActivationEmailAsync(string email, string username, string activationToken);
        Task SendPasswordResetEmailAsync(string email, string username, string resetToken);
    }
}
