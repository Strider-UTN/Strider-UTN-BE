namespace StriderWebApi.Dto.UserCreation
{
    public class CreateAthleteDto
    {
        public required string Username { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public DateTime BirthDate { get; set; }
        public double HeightCm { get; set; }
        public double WeightKg { get; set; }
        public required string Country { get; set; }
    }
}
