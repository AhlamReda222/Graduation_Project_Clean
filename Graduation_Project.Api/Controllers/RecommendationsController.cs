// FILE: Api/Controllers/RecommendationsController.cs
using Graduation_Project.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace Graduation_Project.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationsController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;

        public RecommendationsController(IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        // ══════════════════════════════════════════════════
        // GET /api/recommendations/product?product_id=45&limit=5
        // الفرونت يستدعيه لما يفتح صفحة منتج
        // بيرجع منتجات مشابهة
        // ══════════════════════════════════════════════════
        [HttpGet("product")]
        public async Task<IActionResult> GetSimilarProducts(
            [FromQuery] int product_id,
            [FromQuery] int limit = 10)
        {
            if (product_id <= 0)
                return BadRequest(new { message = "product_id is required" });

            if (limit is < 1 or > 50)
                limit = 10;

            var result = await _recommendationService.GetSimilarProductsAsync(product_id, limit);

            if (!result.Succeeded)
                return Ok(new
                {
                    status          = "unavailable",
                    product_id,
                    total_results   = 0,
                    similar_products = Array.Empty<object>(),
                    message         = result.Message
                });

            return Ok(result.Data);
        }

        // ══════════════════════════════════════════════════
        // GET /api/recommendations/home?limit=10
        // الفرونت يستدعيه لما يفتح الصفحة الرئيسية
        // بيرجع personalized لو في history، popular لو جديد
        // ══════════════════════════════════════════════════
        [HttpGet("home")]
        public async Task<IActionResult> GetHomeRecommendations(
            [FromQuery] int limit = 10)
        {
            // ✅ اجيب الـ userId من الـ JWT
            int? userId = null;
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null && int.TryParse(claim.Value, out var parsed))
                userId = parsed;

            if (userId == null)
                return Unauthorized(new { message = "Login required for personalized recommendations" });

            if (limit is < 1 or > 50)
                limit = 10;

            var result = await _recommendationService.GetHomeRecommendationsAsync(userId.Value, limit);

            if (!result.Succeeded)
                return Ok(new
                {
                    status          = "unavailable",
                    user_id         = userId,
                    type            = "popular",
                    recommendations = Array.Empty<object>(),
                    message         = result.Message
                });

            return Ok(result.Data);
        }
    }
}