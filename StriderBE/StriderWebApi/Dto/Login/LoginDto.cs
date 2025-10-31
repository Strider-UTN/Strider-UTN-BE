using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Dto.Login
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public UserTypeEnum UserType { get; set; }
    }
}
