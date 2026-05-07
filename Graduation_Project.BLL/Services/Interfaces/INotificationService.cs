using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.Notification;

namespace Graduation_Project.BLL.Services.Interfaces
{
    public interface INotificationService
    {
        Task<ServiceResult<NotificationDto>> CreateNotificationAsync(int userId, string title, string message);
        Task<ServiceResult<IEnumerable<NotificationDto>>> GetMyNotificationsAsync(int userId);
        Task<ServiceResult<bool>> MarkAsReadAsync(int notificationId, int userId);
        Task<ServiceResult<bool>> MarkAllAsReadAsync(int userId);
    }
}
