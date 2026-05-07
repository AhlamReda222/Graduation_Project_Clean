using Graduation_Project.BLL.DTOs.BrandOwnerRequest;
using Graduation_Project.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Graduation_Project.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BrandOwnerRequestController : ControllerBase
    {
        private readonly IBrandOwnerRequestService _service;

        public BrandOwnerRequestController(IBrandOwnerRequestService service)
        {
            _service = service;
        }

        // ✅ Create Request (Files + FormData)
        [HttpPost("request-owner")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> CreateRequest([FromForm] CreateBrandOwnerRequestDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _service.CreateRequestAsync(userId, dto);

            return Ok(result);
        }

        // ✅ Admin - Get All Requests
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllRequestsAsync();
            return Ok(result);
        }

        // ✅ Admin - Get Pending Requests
        [HttpGet("pending")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetPending()
        {
            var result = await _service.GetPendingRequestsAsync();
            return Ok(result);
        }

        // ✅ Admin - Approve Request
        [HttpPut("{requestId}/approve")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Approve(int requestId)
        {
            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _service.ApproveRequestAsync(requestId, adminId);

            return Ok(result);
        }

        // ✅ Admin - Reject Request
        [HttpPut("{requestId}/reject")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Reject(int requestId)
        {
            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _service.RejectRequestAsync(requestId, adminId);

            return Ok(result);
        }

        // ✅ Customer - My Requests
        [HttpGet("my-requests")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> GetMyRequests()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _service.GetUserRequestsAsync(userId);

            return Ok(result);
        }

        // ✅ Admin - Delete Request
        [HttpDelete("{requestId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int requestId)
        {
            var result = await _service.DeleteRequestAsync(requestId);

            if (!result)
                return NotFound(new { message = "Request not found" });

            return Ok(new { message = "Deleted successfully" });
        }
    }
}