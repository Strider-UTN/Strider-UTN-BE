using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Dto.Login
{
    public class GoogleLoginDto
    {
        public string IdToken { get; set; } = string.Empty;
        public UserTypeEnum UserType { get; set; }
    }
}
