  namespace Graduation_Project.BLL.DTOs.Recommendation
{
public class HomeRecommendationResponseDto
    {
        public string Status { get; set; } = null!;
        public int UserId { get; set; }
        public string Type { get; set; } = null!; // "personalized" أو "popular"
        public List<RecommendedProductDto> Recommendations { get; set; } = new();
    }}