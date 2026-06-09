using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.Auth;
using Graduation_Project.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Graduation_Project.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
[HttpPost("login")]
public async Task<IActionResult> Login(LoginDto dto)
{
    var sessionId =
        Request.Headers["X-Session-Id"]
        .FirstOrDefault();

    var result =
        await _authService.LoginAsync(
            dto,
            sessionId);

    return Ok(result);
}
 

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
        {
            var result = await _authService.GoogleLoginAsync(dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        // ── Forgot Password: اليوزر بيدخل الإيميل ويستقبل Code ──
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            await _authService.ForgotPasswordAsync(dto);
            // دايماً بنرجع Ok عشان مش نقول للـ hacker إن الإيميل موجود أو لأ
            return Ok(new { message = "If this email exists, a reset code has been sent." });
        }

        // ── Reset Password: اليوزر بيدخل الـ Code والـ Password الجديد ──
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var result = await _authService.ResetPasswordAsync(dto);
            if (!result)
                return BadRequest(new { message = "Invalid or expired code." });

            return Ok(new { message = "Password reset successfully." });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            var result = await _authService.RefreshTokenAsync(dto.RefreshToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutDto dto)
        {
            await _authService.LogoutAsync(dto.RefreshToken);
            return Ok(new { message = "Logged out successfully" });
        }
    }
}