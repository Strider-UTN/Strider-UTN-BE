using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Model;

public abstract class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime BirthDate { get; set; }
    public Gender Gender { get; set; }
    public required string Address { get; set; }
    public bool Active { get; set; }
    public string? ActivationToken { get; set; }
    public DateTime? ActivationTokenExpires { get; set; }
    public UserTypeEnum Type { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public required string CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public DateTime CreatedDate { get; set; }
}
