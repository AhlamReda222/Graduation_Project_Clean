using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.Recommendation;
 
namespace Graduation_Project.BLL.Services.Interfaces
{
    public interface IRecommendationService
    {
        // 1. بعت activity للـ AI عشان يتعلم
        Task TrackActivityAsync(int? userId, int? productId, string actionType, string? searchQuery = null);
 
        // 2. منتجات مشابهة لمنتج معين
        Task<ServiceResult<ProductRecommendationResponseDto>> GetSimilarProductsAsync(int productId, int limit = 10);
 
        // 3. ترشيحات الصفحة الرئيسية للـ User
        Task<ServiceResult<HomeRecommendationResponseDto>> GetHomeRecommendationsAsync(int userId, int limit = 10);
 
        // 4. Refresh الـ model لما يتضاف/يتعدل/يتحذف منتج
        Task RefreshRecommendationsAsync();
    }
}
 




































































