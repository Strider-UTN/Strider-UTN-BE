using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Dto.Login
{
    public class ForgotPasswordDto
    {
        public required string Email { get; set; }
        public UserTypeEnum UserType { get; set; }
    }
}
