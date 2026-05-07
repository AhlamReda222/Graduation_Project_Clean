using Graduation_Project.BLL.DTOs.Product;
using Graduation_Project.BLL.Services.Interfaces;
using Graduation_Project.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
 
namespace Graduation_Project.API.Controllers
{
    [ApiController]
    [Route("api/product-ai")]
    [Authorize(Policy = "BrandOwnerOnly")]
    public class ProductAIController : ControllerBase
    {
        private readonly IProductAIService _productAIService;
        private readonly IUnitOfWork _unitOfWork;
 
        public ProductAIController(
            IProductAIService productAIService,
            IUnitOfWork unitOfWork)
        {
            _productAIService = productAIService;
            _unitOfWork = unitOfWork;
        }
 
        // ============================================================
        // 1. IMAGE VALIDATION (Real-time)
        // POST api/product-ai/validate-image
        // الفرونت يستدعيه فور ما الأونر يختار الصورة
        // ============================================================
        [HttpPost("validate-image")]
        public async Task<IActionResult> ValidateImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded" });
 
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp", "image/jpg" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
                return BadRequest(new { message = "Only JPG, PNG, WEBP images are allowed" });
 
            if (file.Length > 5 * 1024 * 1024)
                return BadRequest(new { message = "Image size must be less than 5MB" });
 
            var result = await _productAIService.ValidateImageAsync(file);
 
            return Ok(new
            {
                isValid      = result.IsValid,
                status       = result.Status,
                message      = result.Message,
                aiConfidence = result.AiConfidence,
                details      = result.Details
            });
        }
 
        // ============================================================
        // 2. PRICE PREDICTION (Real-time)
        // POST api/product-ai/predict-price
        // الفرونت يستدعيه لما الأونر يكمل الاسم والوصف
        // ============================================================
        [HttpPost("predict-price")]
        public async Task<IActionResult> PredictPrice(
            [FromForm] string productName,
            [FromForm] string description,
            [FromForm] string category,
            [FromForm] decimal basePrice,
            IFormFile? file)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return BadRequest(new { message = "Product name is required" });
 
            if (string.IsNullOrWhiteSpace(description))
                return BadRequest(new { message = "Description is required" });
 
            // جيب اسم الـ Brand
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
 
            var brand = await _unitOfWork.Brands
                .GetQueryable()
                .Where(b => b.UserId == userId && b.IsActive)
                .FirstOrDefaultAsync();
 
            var brandName = brand?.BrandName ?? "";
 
            try
            {
                var result = await _productAIService.PredictPriceAsync(
                    productName,
                    description,
                    category,
                    basePrice,
                    brandName,
                    file
                );
 
                if (result?.PricePrediction == null)
                    return Ok(new
                    {
                        status  = "unavailable",
                        message = "Could not predict price, please set manually"
                    });
 
                return Ok(new
                {
                    status         = result.Status,
                    suggestedPrice = result.PricePrediction.SuggestedPrice,
                    minPrice       = result.PricePrediction.PriceRange?.Min,
                    maxPrice       = result.PricePrediction.PriceRange?.Max,
                    reasoning      = result.PricePrediction.Reasoning,
                    note           = result.PriceNote
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status  = "error",
                    message = $"Could not predict price: {ex.Message}"
                });
            }
        }
 
        // ============================================================
        // 3. TEXT MODERATION (Real-time)
        // POST api/product-ai/moderate-text
        // الفرونت يستدعيه لما الأونر يكتب الاسم والوصف
        // ============================================================
        [HttpPost("moderate-text")]
        public async Task<IActionResult> ModerateText([FromBody] ModerateTextRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ProductName))
                return BadRequest(new { message = "Product name is required" });
 
            if (string.IsNullOrWhiteSpace(dto.Description))
                return BadRequest(new { message = "Description is required" });
 
            try
            {
                var result = await _productAIService.ModerateTextAsync(
                    dto.ProductName,
                    dto.Description,
                    dto.Category,
                    dto.Price
                );
 
                return Ok(new
                {
                    isApproved = result.IsApproved,
                    status     = result.Status,
                    message    = result.Message,
                    reason     = result.Reason
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    isApproved = false,
                    status     = "error",
                    message    = $"Moderation check failed: {ex.Message}"
                });
            }
        }
    }
}