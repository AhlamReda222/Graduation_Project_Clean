using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.Notification;
using Graduation_Project.BLL.Services.Interfaces;
using Graduation_Project.DAL.Models.Entities;
using Graduation_Project.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Graduation_Project.BLL.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<NotificationDto>> CreateNotificationAsync(int userId, string title, string message)
        {
            try
            {
                var notification = new Notification
                {
                    UserId = userId,
                    Title = title,
                    Message = message,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                };

                await _unitOfWork.Notifications.AddAsync(notification);
                await _unitOfWork.SaveAsync();

                return ServiceResult<NotificationDto>.Success(MapToDto(notification));
            }
            catch (Exception ex)
            {
                return ServiceResult<NotificationDto>.Failure($"Error creating notification: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<NotificationDto>>> GetMyNotificationsAsync(int userId)
        {
            try
            {
                var notifications = await _unitOfWork.Notifications
                    .GetQueryable()
                    .Where(n => n.UserId == userId)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync();

                return ServiceResult<IEnumerable<NotificationDto>>.Success(
                    notifications.Select(MapToDto));
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<NotificationDto>>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> MarkAsReadAsync(int notificationId, int userId)
        {
            try
            {
                var notification = await _unitOfWork.Notifications
                    .FindOneAsync(n => n.NotificationId == notificationId && n.UserId == userId);

                if (notification == null)
                    return ServiceResult<bool>.Failure("Notification not found.");

                notification.IsRead = true;
                _unitOfWork.Notifications.Update(notification);
                await _unitOfWork.SaveAsync();

                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> MarkAllAsReadAsync(int userId)
        {
            try
            {
                var unread = await _unitOfWork.Notifications
                    .FindAsync(n => n.UserId == userId && !n.IsRead);

                foreach (var n in unread)
                {
                    n.IsRead = true;
                    _unitOfWork.Notifications.Update(n);
                }

                await _unitOfWork.SaveAsync();

                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure($"Error: {ex.Message}");
            }
        }

        private NotificationDto MapToDto(Notification n) => new NotificationDto
        {
            NotificationId = n.NotificationId,
            Title = n.Title,
            Message = n.Message,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt
        };
    }
}   
