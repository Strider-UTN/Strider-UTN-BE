using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace StriderWebApi.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string user, string message)
        {
            await Clients.User(user).SendAsync("ReceiveNotification", message);
        }

        public override Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier; // debería tener el userId como string
            Console.WriteLine($"Usuario conectado: {userId}");
            return base.OnConnectedAsync();
        }
    }
}
