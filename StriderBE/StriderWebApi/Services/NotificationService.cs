using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Exceptions.Notification;
using StriderWebApi.Services.Interfaces;

namespace StriderWebApi.Services
{
    public class NotificationService(INotificationRepository repository) : INotificationService
    {
        private readonly INotificationRepository _repository = repository;

        public async Task<List<Notification>> GetAllForUserAsync(int userId)
        {
            return await _repository.GetByUserIdAsync(userId);
        }

        public async Task<List<Notification>> GetUnreadForUserAsync(int userId)
        {
            return await _repository.GetUnreadByUserIdAsync(userId);
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _repository.GetByIdAsync(notificationId) ?? throw new NotificationNotFoundException("No hemos podido encontrar la notificación con el Id proporcionado.");

            if (notification.IsRead) throw new NotificationAlreadyMarkedAsReadException("La notificación ya se encuentra marcada como leída");

            await _repository.MarkAsReadAsync(notification);
        }

        public async Task DeleteAsync(int notificationId)
        {
            var notification = await _repository.GetByIdAsync(notificationId) ?? throw new NotificationNotFoundException("No hemos podido encontrar la notificación con el Id proporcionado.");

            await _repository.RemoveAsync(notification);
        }
    }
}
