namespace StriderWebApi.Dto.UserCreation
{
    public class ActivateAccountDto
    {
        public int UserId { get; set; }
        public string ActivationToken { get; set; } = string.Empty;
    }
}
