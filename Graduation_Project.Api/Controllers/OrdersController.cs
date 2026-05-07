using Graduation_Project.BLL.DTOs.Order;
using Graduation_Project.BLL.Services.Interfaces;
using Graduation_Project.DAL.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Graduation_Project.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // Customer يعمل Order ويدفع في نفس الوقت
        [HttpPost]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _orderService.CreateOrderFromCartAsync(userId, dto);
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok(result.Data);
        }

        // Customer يشوف Orders بتاعته
        [HttpGet("my-orders")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _orderService.GetMyOrdersAsync(userId);
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok(result.Data);
        }

        // Customer يشوف Order معين
        [HttpGet("{orderId}")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _orderService.GetOrderByIdAsync(orderId, userId);
            if (!result.Succeeded) return NotFound(result.Errors);
            return Ok(result.Data);
        }

        // Customer يكنسل Order
        [HttpPut("{orderId}/cancel")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _orderService.CancelOrderAsync(orderId, userId);
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok(result.Data);
        }

        // owner يشوف كل الـ Orders
        [HttpGet]
        [Authorize(Policy = "BrandOwnerOnly")]
        public async Task<IActionResult> GetAllOrders()
        {
            var result = await _orderService.GetAllOrdersAsync();
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok(result.Data);
        }

        // Admin يعدل Status
        [HttpPut("{orderId}/status")]
        [Authorize(Policy = "BrandOwnerOnly")]
        public async Task<IActionResult> UpdateStatus(int orderId, [FromQuery] OrderStatus status)
        {
            var result = await _orderService.UpdateOrderStatusAsync(orderId, status);
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok(result.Data);
        }
    }
}