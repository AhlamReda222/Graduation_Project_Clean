using Graduation_Project.BLL.DTOs.Profile;
using Graduation_Project.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Graduation_Project.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        // GET: api/profile
        // جلب الـ Profile بتاع اليوزر الحالي
        [HttpGet]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _profileService.GetMyProfileAsync(userId);

            if (!result.Succeeded)
                return NotFound(result.Errors);

            return Ok(result.Data);
        }

        // PUT: api/profile
        // تعديل الـ Profile
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _profileService.UpdateProfileAsync(userId, dto);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(result.Data);
        }

        // PUT: api/profile/image
        // تغيير صورة الـ Profile
        [HttpPut("image")]
        public async Task<IActionResult> UpdateProfileImage([FromBody] UpdateProfileImageDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _profileService.UpdateProfileImageAsync(userId, dto);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(result.Data);
        }
    }
}