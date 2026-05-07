using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graduation_Project.BLL.Services.Interfaces;

namespace Graduation_Project.BLL.Common
{
    /// <summary>
    /// بيتستخدم في أي Service عايز يبعت Notification
    /// بيحفظها في DB + بيبعتها Real-time بـ SignalR
    /// </summary>
    public class NotificationHelper
    {
        private readonly INotificationService _notificationService;
        private readonly INotificationHub _notificationHub;

        public NotificationHelper(
            INotificationService notificationService,
            INotificationHub notificationHub)
        {
            _notificationService = notificationService;
            _notificationHub = notificationHub;
        }

        public async Task SendAsync(int userId, string title, string message)
        {
            // 1. احفظ في DB
            var result = await _notificationService.CreateNotificationAsync(userId, title, message);

            // 2. ابعت Real-time
            if (result.Succeeded)
                await _notificationHub.SendNotificationAsync(userId, result.Data);
        }
    }
}