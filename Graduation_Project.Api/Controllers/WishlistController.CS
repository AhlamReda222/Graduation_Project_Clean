using Graduation_Project.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Graduation_Project.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        // GET: api/wishlist
        // جلب كل الـ Wishlist
        [HttpGet]
        public async Task<IActionResult> GetMyWishlist()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _wishlistService.GetMyWishlistAsync(userId);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(result.Data);
        }

        // POST: api/wishlist/{productId}
        // إضافة منتج للـ Wishlist
        [HttpPost("{productId}")]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _wishlistService.AddToWishlistAsync(userId, productId);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(result.Data);
        }

        // DELETE: api/wishlist/{productId}
        // إزالة منتج من الـ Wishlist
        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _wishlistService.RemoveFromWishlistAsync(userId, productId);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Removed from wishlist." });
        }

        // GET: api/wishlist/check/{productId}
        // تأكد إن المنتج في الـ Wishlist
        [HttpGet("check/{productId}")]
        public async Task<IActionResult> IsInWishlist(int productId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _wishlistService.IsInWishlistAsync(userId, productId);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { isInWishlist = result.Data });
        }

        // DELETE: api/wishlist/clear
        // مسح كل الـ Wishlist
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearWishlist()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _wishlistService.ClearWishlistAsync(userId);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Wishlist cleared." });
        }
    }
}