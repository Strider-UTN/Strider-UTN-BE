using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Model;

public class Notification
{ 
    public string? Title { get; set; }
    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;

    public NotificationType Type { get; set; }
    public string? LinkUrl { get; set; }
}