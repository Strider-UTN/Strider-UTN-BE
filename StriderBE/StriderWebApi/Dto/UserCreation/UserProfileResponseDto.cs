using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Dto.UserCreation
{
    public class UserProfileResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Address { get; set; }
        public Gender? Gender { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public UserTypeEnum UserType { get; set; }
        public ThemePreference PreferredTheme { get; set; }

        public string? Bio { get; set; }

        // Campos específicos para atletas
        public double? Height { get; set; }
        public double? Weight { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? EmergencyContactRelationship { get; set; }
        public string? Country { get; set; }
        public string? VO2Max { get; set; } // Velocidad máxima por km en formato mm:ss (ejemplo: "03:30")
        public int YearsOfExperience { get; set; }
        public string? TrainingStartDate { get; set; } // Formato: YYYY-MM
        public TrainingVolumeType TrainingVolumeType { get; set; }
        public int TrainingVolumeKm { get; set; }
    }
}

