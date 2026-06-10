using Graduation_Project.BLL.DTOs.Auth;
using Graduation_Project.BLL.Services.Interfaces;
using Graduation_Project.DAL.Models.Entities;
using Graduation_Project.DAL.Models.Enums;
using Graduation_Project.DAL.Repositories.Interfaces;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;

namespace Graduation_Project.BLL.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IEmailSender _emailSender;
        private readonly IUserBehaviorService _userBehaviorService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            IEmailSender emailSender,
            IUserBehaviorService userBehaviorService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _emailSender = emailSender;
            _userBehaviorService = userBehaviorService;
        }

        // ── Register ──
        public async Task<AuthResponseDto> RegisterAsync(RegisterDto model)
        {
            var existing = await _userManager.FindByEmailAsync(model.Email);
            if (existing != null)
                return Fail("Email already exists");

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                UserType = UserType.Customer,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsBlocked = false
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return Fail(string.Join(", ", result.Errors.Select(e => e.Description)));

            // ✅ إنشاء Profile تلقائي
            await CreateProfileAsync(user.Id);

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "User registered successfully",
                Email = user.Email,
                UserType = user.UserType.ToString()
            };
        }

        // ── Login ──
        public async Task<AuthResponseDto> LoginAsync(LoginDto model,  string? sessionId)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || user.IsBlocked)
                return Fail("Invalid credentials or blocked");

            var passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!passwordCheck.Succeeded)
                return Fail("Invalid credentials");
                  if (!string.IsNullOrEmpty(sessionId))
        await _userBehaviorService.LinkSessionToUserAsync(sessionId, user.Id);
 

            return await GenerateTokensAndReturn(user, "Login successful");
        }

        // ── Google Login ──
        public async Task<AuthResponseDto> GoogleLoginAsync(GoogleLoginDto dto)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken);
                if (payload == null)
                    return Fail("Invalid Google token");

                var user = await _userManager.FindByEmailAsync(payload.Email);

                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = payload.Email,
                        Email = payload.Email,
                        FullName = payload.Name,
                        UserType = UserType.Customer,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsBlocked = false,
                        EmailConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded)
                        return Fail(string.Join(", ", result.Errors.Select(e => e.Description)));

                    // ✅ إنشاء Profile تلقائي للـ Google User
                    await CreateProfileAsync(user.Id);
                }
                else if (user.IsBlocked)
                    return Fail("Your account has been blocked.");

                return await GenerateTokensAndReturn(user, "Google login successful");
            }
            catch (InvalidJwtException)
            {
                return Fail("Invalid or expired Google token.");
            }
            catch (Exception ex)
            {
                return Fail($"Google login failed: {ex.Message}");
            }
        }

        // ── Forgot Password ──
        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return true;

            var oldCodes = await _unitOfWork.PasswordResetCodes
                .FindAsync(c => c.UserId == user.Id && !c.IsUsed);

            foreach (var old in oldCodes)
            {
                old.IsUsed = true;
                _unitOfWork.PasswordResetCodes.Update(old);
            }

            var code = new Random().Next(100000, 999999).ToString();

            await _unitOfWork.PasswordResetCodes.AddAsync(new PasswordResetCode
            {
                UserId = user.Id,
                Code = code,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                IsUsed = false
            });

            await _unitOfWork.SaveAsync();

            var body = $@"
<h2>Reset Your Password</h2>
<p>Your password reset code is:</p>
<h1 style='color:#4CAF50; letter-spacing:5px;'>{code}</h1>
<p>This code is valid for <strong>15 minutes</strong>.</p>
<p>If you didn't request this, please ignore this email.</p>";

            await _emailSender.SendEmailAsync(dto.Email, "🔐 Password Reset Code", body);

            return true;
        }

        // ── Reset Password ──
        public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return false;

            var resetCode = (await _unitOfWork.PasswordResetCodes
                .FindAsync(c => c.UserId == user.Id
                             && c.Code == dto.Code
                             && !c.IsUsed
                             && c.ExpiresAt > DateTime.UtcNow))
                .FirstOrDefault();

            if (resetCode == null) return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

            if (!result.Succeeded) return false;

            resetCode.IsUsed = true;
            _unitOfWork.PasswordResetCodes.Update(resetCode);
            await _unitOfWork.SaveAsync();

            return true;
        }

        // ── Refresh Token ──
        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var tokenEntity = (await _unitOfWork.RefreshTokens
                .FindAsync(r => r.Token == refreshToken))
                .FirstOrDefault();

            if (tokenEntity == null || tokenEntity.IsRevoked || tokenEntity.Expires < DateTime.UtcNow)
                return Fail("Invalid or expired refresh token");

            tokenEntity.IsRevoked = true;
            _unitOfWork.RefreshTokens.Update(tokenEntity);

            var user = await _userManager.FindByIdAsync(tokenEntity.ApplicationUserId.ToString());
            if (user == null || user.IsBlocked)
                return Fail("User not found or blocked");

            return await GenerateTokensAndReturn(user, "Token refreshed successfully");
        }

        // ── Logout ──
        public async Task<bool> LogoutAsync(string refreshToken)
        {
            var tokenEntity = (await _unitOfWork.RefreshTokens
                .FindAsync(r => r.Token == refreshToken))
                .FirstOrDefault();

            if (tokenEntity != null && !tokenEntity.IsRevoked)
            {
                tokenEntity.IsRevoked = true;
                _unitOfWork.RefreshTokens.Update(tokenEntity);
                await _unitOfWork.SaveAsync();
            }

            return true;
        }

        // ── Helper: إنشاء Profile تلقائي ──
        private async Task CreateProfileAsync(int userId)
        {
            var profile = new Profile
            {
                UserId = userId,
                ProfileImage = "",  
                Address = "",       
                Bio = "",           
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Profiles.AddAsync(profile);
            await _unitOfWork.SaveAsync();
        }
        // ── Helper: توليد Tokens ──
        private async Task<AuthResponseDto> GenerateTokensAndReturn(ApplicationUser user, string message)
        {
            var oldTokens = await _unitOfWork.RefreshTokens
                .FindAsync(r => r.ApplicationUserId == user.Id && !r.IsRevoked);

            foreach (var token in oldTokens)
            {
                token.IsRevoked = true;
                _unitOfWork.RefreshTokens.Update(token);
            }

            var jwt = _tokenService.GenerateJwtToken(user);
            var refresh = _tokenService.GenerateRefreshToken();

            await _unitOfWork.RefreshTokens.AddAsync(new RefreshToken
            {
                Token = refresh,
                Expires = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
                ApplicationUserId = user.Id
            });

            await _unitOfWork.SaveAsync();

            return Success(jwt, refresh, user, message);
        }

        private AuthResponseDto Fail(string message)
            => new AuthResponseDto { IsSuccess = false, Message = message };

        private AuthResponseDto Success(string jwt, string refresh, ApplicationUser user, string message)
            => new AuthResponseDto
            {

                IsSuccess = true,
                Message = message,
                Token = jwt,
                RefreshToken = refresh,
                Email = user.Email,
                UserType = user.UserType.ToString(),
                 UserId = user.Id,

            };
    }
}