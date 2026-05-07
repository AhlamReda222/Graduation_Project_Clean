using Graduation_Project.BLL.DTOs.Product;
using Graduation_Project.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
 
namespace Graduation_Project.BLL.Services.Implementations
{
    public class ProductAIService : IProductAIService
    {
        private readonly IAiModerationService _ai;
 
        public ProductAIService(IAiModerationService ai)
        {
            _ai = ai;
        }
 
        // ✅ 1. IMAGE VALIDATION (Real-time)
        // الفرونت يبعت الصورة فور ما الأونر يختارها
        public Task<ImageValidationResultDto> ValidateImageAsync(IFormFile file)
            => _ai.ValidateImageAsync(file);
 
        // ✅ 2. PRICE PREDICTION (Real-time)
        // الفرونت يبعت لما يكمل الاسم والوصف والصورة
        public async Task<AiPredictionResultDto> PredictPriceAsync(
            string productName,
            string description,
            string category,
            decimal basePrice,
            string? brandName,
            IFormFile? image)
        {
            var request = new AiModerationRequestDto
            {
                ProductName = productName,
                Description = description,
                Category = category,
                Price = basePrice
            };
 
            return await _ai.PredictProductAsync(request, image, brandName);
        }
 
        // ✅ 3. TEXT MODERATION (Real-time)
        // الفرونت يبعت لما الأونر يكتب الاسم والوصف
        public async Task<AiModerationResultDto> ModerateTextAsync(
            string productName,
            string description,
            string category,
            decimal price)
        {
            var request = new AiModerationRequestDto
            {
                ProductName = productName,
                Description = description,
                Category = category,
                Price = price
            };
 
            return await _ai.ModerateProductAsync(request);
        }
    }
}
 