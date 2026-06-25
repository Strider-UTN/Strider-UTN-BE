using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Domain.DomainClasses
{
    public abstract class User
    {
        public int Id { get; set; }

        public required string FullName { get; set; }
        public required string Email { get; set; }
        public string PhoneNumber { get; set; } = string.Empty; // Default to empty if not provided
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public required string Address { get; set; }
        public string? PasswordHash { get; set; }
        public bool Active { get; set; }
        public string? ActivationToken { get; set; }
        public DateTime? ActivationTokenExpires { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpires { get; set; }
        public ThemePreference PreferredTheme { get; set; } = ThemePreference.Light; // Default claro

        public UserTypeEnum UserType { get; set; }

        public string? ProfilePictureUrl { get; set; }
        public string CreatedBy { get; set; } = "System"; // Default to System for initial creation
        public string? UpdatedBy { get; set; } // Nullable to allow for initial creation without updates
        public DateTime? UpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }

        // Sobre ti / biografía breve
        public string? Bio { get; set; }
    }
}
