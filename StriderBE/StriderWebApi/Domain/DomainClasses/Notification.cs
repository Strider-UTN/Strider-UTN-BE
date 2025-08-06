using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Domain.DomainClasses
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string? Title { get; set; }
        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;

        public NotificationType Type { get; set; }
        public string? LinkUrl { get; set; }

        // Navigation properties
        public virtual User? User { get; set; }
    }
}
