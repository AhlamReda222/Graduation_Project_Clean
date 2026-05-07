using Graduation_Project.BLL.DTOs.Product;
using Microsoft.AspNetCore.Http;
 
namespace Graduation_Project.BLL.Services.Interfaces
{
    public interface IAiModerationService
    {
        Task<ImageValidationResultDto> ValidateImageAsync(IFormFile file);
 
        Task<AiPredictionResultDto> PredictProductAsync(
            AiModerationRequestDto request,
            IFormFile? file,
            string? brandName);
 
        Task<AiModerationResultDto> ModerateProductAsync(AiModerationRequestDto request);
    }
}