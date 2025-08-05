using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Model;

public abstract class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public Gender Gender { get; set; }
    public string Address { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
    public bool Active { get; set; }
    public string? ActivationToken { get; set; }
    public DateTime? ActivationTokenExpires { get; set; }
    public UserTypeEnum Type { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string CreatedBy { get; set; } = "System";
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public DateTime CreatedDate { get; set; }
}