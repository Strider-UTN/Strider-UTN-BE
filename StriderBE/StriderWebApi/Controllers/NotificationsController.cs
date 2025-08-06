using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Exceptions.Notification;
using StriderWebApi.Hubs;
using StriderWebApi.Services.Interfaces;

namespace StriderWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController(IHubContext<NotificationHub> hubContext, INotificationService notificationService) : ControllerBase
    {
        private readonly IHubContext<NotificationHub> _hubContext = hubContext;
        private readonly INotificationService _notificationService = notificationService;

        [HttpPost("Send")]
        public async Task<IActionResult> SendNotification([FromQuery] int targetUserId)
        {
            var senderUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            await _hubContext.Clients
                .User(targetUserId.ToString())
                .SendAsync("ReceiveNotification", $"Notificación de usuario {senderUserId}");

            return Ok("Notificación enviada");
        }

        [HttpGet("User/{userId:int}")]
        public async Task<ActionResult<List<Notification>>> GetAll(int userId)
        {
            try
            {
                var notifications = await _notificationService.GetAllForUserAsync(userId);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return Problem($"Error al obtener notificaciones: {ex.Message}");
            }
        }

        [HttpGet("User/{userId:guid}/Unread")]
        public async Task<ActionResult<List<Notification>>> GetUnread(int userId)
        {
            try
            {
                var notifications = await _notificationService.GetUnreadForUserAsync(userId);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return Problem($"Error al obtener notificaciones no leídas: {ex.Message}");
            }
        }

        [HttpPost("{id:int}/MarkAsRead")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                await _notificationService.MarkAsReadAsync(id);

                return Ok();
            }
            catch (NotificationAlreadyMarkedAsReadException ex)
            {
                return BadRequest($"Notificación ya estaba marcada como leída: {ex.Message}");
            }
            catch (NotificationNotFoundException ex)
            {
                return NotFound($"Notificación no encontrada: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Problem($"Error al marcar la notificación como leída: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _notificationService.DeleteAsync(id);

                return Ok();
            }
            catch (NotificationNotFoundException ex)
            {
                return NotFound($"Notificación no encontrada: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Problem($"Error al eliminar la notificación: {ex.Message}");
            }
        }

    }
}
