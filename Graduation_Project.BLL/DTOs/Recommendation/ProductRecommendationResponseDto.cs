namespace Graduation_Project.BLL.DTOs.Recommendation
{
    // الـ Response من /recommendations/product
    public class ProductRecommendationResponseDto
    {
        public string Status { get; set; } = null!;
        public int ProductId { get; set; }
        public int TotalResults { get; set; }
        public List<RecommendedProductDto> SimilarProducts { get; set; } = new();
    }}
 
    // الـ Response من /recommendations/home
  
