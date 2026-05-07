using Graduation_Project.BLL.DTOs.Product;
using Microsoft.AspNetCore.Http;
 
namespace Graduation_Project.BLL.Services.Interfaces
{
    public interface IProductAIService
    {
        // 1. صورة فقط (Real-time لما الأونر يرفع صورة)
        Task<ImageValidationResultDto> ValidateImageAsync(IFormFile file);
 
        // 2. اسم + وصف + صورة → اقتراح السعر (Real-time)
        Task<AiPredictionResultDto> PredictPriceAsync(
            string productName,
            string description,
            string category,
            decimal basePrice,
            string? brandName,
            IFormFile? image);
 
        // 3. اسم + وصف فقط → فحص النصوص (Real-time)
        Task<AiModerationResultDto> ModerateTextAsync(
            string productName,
            string description,
            string category,
            decimal price);
    }
}
 