using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Dto.User
{
    public class UserSummaryDto
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public UserTypeEnum UserType { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpires { get; set; }
    }
}
