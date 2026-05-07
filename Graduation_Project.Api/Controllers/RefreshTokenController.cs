using Graduation_Project.BLL.DTOs.Auth;
using Graduation_Project.DAL.DataBase;
using Graduation_Project.DAL.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Graduation_Project.BLL.Services.Interfaces;

namespace Graduation_Project.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefreshTokenController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;

        public RefreshTokenController(
            ApplicationDbContext context,
            ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost]
        public async Task<IActionResult> RefreshTokenAsync(
            [FromBody] RefreshTokenDto request)
        {
            var existingToken = await _context.RefreshTokens
                .Include(r => r.User)
                .SingleOrDefaultAsync(r => r.Token == request.RefreshToken);

            if (existingToken == null ||
                existingToken.IsRevoked ||
                existingToken.Expires < DateTime.UtcNow)
            {
                return Unauthorized("Expired or Invalid refresh token.");
            }

            var user = existingToken.User;

            // Revoke old refresh token
            existingToken.IsRevoked = true;

            // Generate new tokens
            var newJwt = _tokenService.GenerateJwtToken(user);
            var newRefreshTokenValue = _tokenService.GenerateRefreshToken();

            var newRefreshToken = new RefreshToken
            {
                Token = newRefreshTokenValue,
                Expires = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
                ApplicationUserId = user.Id
            };

            _context.RefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync();

            var response = new TokenResponseDto
            {
                Token = newJwt,
                RefreshToken = newRefreshTokenValue
            };

            return Ok(response);
        }
    }
}
