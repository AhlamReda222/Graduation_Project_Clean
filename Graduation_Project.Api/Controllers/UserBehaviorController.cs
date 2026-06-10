// FILE: Api/Controllers/UserBehaviorController.cs
using Graduation_Project.BLL.DTOs.Tracking;
using Graduation_Project.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Graduation_Project.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserBehaviorController : ControllerBase
    {
        private readonly IUserBehaviorService _userBehaviorService;
        private const string SessionCookieName = "lb_session";

        public UserBehaviorController(IUserBehaviorService userBehaviorService)
        {
            _userBehaviorService = userBehaviorService;
        }

        // ══════════════════════════════════════════════════════
        // POST /api/userbehavior/track
        // يشتغل بدون Auth - لو مسجل دخول بيربطه بـ userId
        // ══════════════════════════════════════════════════════
        [HttpPost("track")]
        public async Task<IActionResult> TrackEvent([FromBody] TrackUserBehaviorRequest request)
        {
            // ✅ 1. اجيب الـ SessionId من الـ Cookie (اتعمل تلقائي في الـ Middleware)
            var sessionId = HttpContext.Items[SessionCookieName]?.ToString()
                         ?? Request.Cookies[SessionCookieName]
                         ?? Guid.NewGuid().ToString();

            // ✅ 2. Override الـ SessionId دايماً بالـ Cookie مش اللي الفرونت بعته
            request.SessionId = sessionId;

            // ✅ 3. لو الـ User مسجل دخول، اربط الـ event بيه
            int? userId = null;
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null && int.TryParse(claim.Value, out var parsedId))
                userId = parsedId;

            var result = await _userBehaviorService.TrackEventAsync(request, userId);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }
    }
}