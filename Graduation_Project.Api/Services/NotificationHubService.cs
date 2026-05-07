using Graduation_Project.Api.Hubs;
using Graduation_Project.BLL.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Graduation_Project.Api.Services
{
    public class NotificationHubService : INotificationHub
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationHubService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotificationAsync(int userId, object notification)
        {
            await _hubContext.Clients
                .Group($"user_{userId}")
                .SendAsync("ReceiveNotification", notification);
        }
    }
}