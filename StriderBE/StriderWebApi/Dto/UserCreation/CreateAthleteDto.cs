using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Dto.UserCreation
{
    public class CreateAthleteDto
    {
        public required string Username { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public DateTime BirthDate { get; set; }
        public required string Address { get; set; }
        public Gender Gender { get; set; }
        public double HeightCm { get; set; }
        public double WeightKg { get; set; }
        public required string Country { get; set; }
        public int YearsOfExperience { get; set; } = 0; // Default to 0 if not provided
        public TrainingVolumeType VolumeType { get; set; } = TrainingVolumeType.Weekly; // Default to Weekly if not provided
        public int TrainingVolumeKm { get; set; } = 0; // Default to 0 if not provided
        public string EmergencyContactName { get; set; } = string.Empty; // Default to empty if not provided
        public string EmergencyContactPhone { get; set; } = string.Empty; // Default to empty if not provided
        public string EmergencyContactRelationship { get; set; } = string.Empty; // Default to empty if not provided
    }
}
