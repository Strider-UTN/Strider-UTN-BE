using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Dto.UserCreation
{
    public class CreateCoachDto
    {
        public string? Username { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public DateTime BirthDate { get; set; }
        public required string Address { get; set; }
        public Gender Gender { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
