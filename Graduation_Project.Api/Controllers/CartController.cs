using Graduation_Project.BLL.DTOs.Cart;
using Graduation_Project.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Graduation_Project.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "CustomerOnly")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // يشوف الكارت بتاعته
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _cartService.GetCartAsync(userId);
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok(result.Data);
        }

        // يضيف للكارت
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _cartService.AddToCartAsync(userId, dto);
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok(result.Data);
        }

        // يعدل الكمية
        [HttpPut("{cartItemId}")]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, [FromQuery] int quantity)
        {
            if (quantity < 1) return BadRequest("Quantity must be at least 1");
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _cartService.UpdateQuantityAsync(userId, cartItemId, quantity);
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok(result.Data);
        }

        // يشيل item من الكارت
        [HttpDelete("{cartItemId}")]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _cartService.RemoveFromCartAsync(userId, cartItemId);
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok(new { message = result.Message });
        }

        // يمسح الكارت كله
        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _cartService.ClearCartAsync(userId);
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok(new { message = result.Message });
        }
    }
}