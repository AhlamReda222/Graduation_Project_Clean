using Graduation_Project.BLL.DTOs.Tracking;
using Graduation_Project.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Graduation_Project.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrackingController : ControllerBase
    {
        private readonly IUserBehaviorService _userBehaviorService;

        public TrackingController(IUserBehaviorService userBehaviorService)
        {
            _userBehaviorService = userBehaviorService;
        }

        [HttpPost("events")]
        [AllowAnonymous]
        public async Task<IActionResult> TrackEvent([FromBody] TrackUserBehaviorRequest request)
        {
            int? userId = null;

            var userIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                User.FindFirst("sub")?.Value;

            if (int.TryParse(userIdClaim, out var parsedUserId))
                userId = parsedUserId;

            var result = await _userBehaviorService.TrackEventAsync(request, userId);

          if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }
    }
}