using Graduation_Project.BLL.DTOs.Auth;

namespace Graduation_Project.BLL.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto model);
        Task<AuthResponseDto> LoginAsync(LoginDto model, string? sessionId);
        Task<AuthResponseDto> GoogleLoginAsync(GoogleLoginDto dto);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
        Task<bool> LogoutAsync(string refreshToken);

        // ✅ Forgot Password
        Task<bool> ForgotPasswordAsync(ForgotPasswordDto dto);
        Task<bool> ResetPasswordAsync(ResetPasswordDto dto);
    }
}