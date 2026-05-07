using Graduation_Project.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Graduation_Project.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // GET: api/notification
        // جلب كل الـ notifications للمستخدم الحالي
        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _notificationService.GetMyNotificationsAsync(userId);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(result.Data);
        }

        // PUT: api/notification/{id}/read
        // تعليم notification كـ مقروءة
        [HttpPut("{notificationId}/read")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _notificationService.MarkAsReadAsync(notificationId, userId);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Marked as read." });
        }

        // PUT: api/notification/read-all
        // تعليم كل الـ notifications كـ مقروءة
        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _notificationService.MarkAllAsReadAsync(userId);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "All notifications marked as read." });
        }
    }
}