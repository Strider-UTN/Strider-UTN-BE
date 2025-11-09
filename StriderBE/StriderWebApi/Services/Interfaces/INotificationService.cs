using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Services.Interfaces
{
    public interface INotificationService
    {
        Task<List<Notification>> GetAllForUserAsync(int userId);
        Task<List<Notification>> GetUnreadForUserAsync(int userId);
        Task MarkAsReadAsync(int notificationId);
        Task DeleteAsync(int notificationId);
        Task<Notification> CreateAsync(Notification notification, CancellationToken cancellationToken = default);
        Task CreateRangeAsync(IEnumerable<Notification> notifications, CancellationToken cancellationToken = default);
    }
}
