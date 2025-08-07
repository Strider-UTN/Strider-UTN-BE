using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Data.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task<Notification?> GetByIdAsync(int id);
        Task<List<Notification>> GetByUserIdAsync(int userId);
        Task<List<Notification>> GetUnreadByUserIdAsync(int userId);
        Task AddAsync(Notification notification);
        Task AddRangeAsync(IEnumerable<Notification> notifications);
        Task MarkAsReadAsync(Notification notification);
        Task RemoveAsync(Notification notification);
    }
}
