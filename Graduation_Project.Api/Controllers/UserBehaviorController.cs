using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.Tracking;
using Graduation_Project.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Graduation_Project.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserBehaviorController : ControllerBase
    {
        private readonly IUserBehaviorService _userBehaviorService;

        public UserBehaviorController(IUserBehaviorService userBehaviorService)
        {
            _userBehaviorService = userBehaviorService;
        }

        [HttpGet("session")]
        public IActionResult CreateSession()
        {
            return Ok(new
            {
                sessionId = Guid.NewGuid().ToString()
            });
        }

        [HttpPost("track")]
        public async Task<IActionResult> TrackEvent(
            [FromBody] TrackUserBehaviorRequest request)
        {
            int? userId = null;

            var result = await _userBehaviorService.TrackEventAsync(request, userId);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }
    }
}