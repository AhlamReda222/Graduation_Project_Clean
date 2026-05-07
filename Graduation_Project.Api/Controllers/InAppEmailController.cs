using Graduation_Project.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Graduation_Project.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InAppEmailController : ControllerBase
    {
        private readonly IInAppEmailService _emailService;

        public InAppEmailController(IInAppEmailService emailService)
        {
            _emailService = emailService;
        }

        // GET: api/inappemail
        // جلب كل الإيميلات (زي صندوق الوارد)
        [HttpGet]
        public async Task<IActionResult> GetMyEmails()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _emailService.GetMyEmailsAsync(userId);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(result.Data);
        }

        // GET: api/inappemail/{id}
        // فتح إيميل معين — بيتعلم كمقروء تلقائياً
        [HttpGet("{emailId}")]
        public async Task<IActionResult> GetEmail(int emailId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _emailService.GetEmailByIdAsync(emailId, userId);

            if (!result.Succeeded)
                return NotFound(result.Errors);

            return Ok(result.Data);
        }

        // GET: api/inappemail/unread-count
        // عدد الإيميلات الغير مقروءة
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _emailService.GetUnreadCountAsync(userId);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { unreadCount = result.Data });
        }

        // PUT: api/inappemail/{id}/read
        [HttpPut("{emailId}/read")]
        public async Task<IActionResult> MarkAsRead(int emailId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _emailService.MarkAsReadAsync(emailId, userId);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Marked as read." });
        }

        // PUT: api/inappemail/read-all
        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _emailService.MarkAllAsReadAsync(userId);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "All emails marked as read." });
        }
    }
}