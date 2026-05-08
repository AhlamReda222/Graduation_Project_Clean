using Graduation_Project.DAL.Models.Enums;

namespace Graduation_Project.BLL.DTOs.Product
{
    public class ProductDto
    {
        public int ProductId { get; set; }
                public int UserId { get; set; }

        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string ImageUrls { get; set; }
        public bool AllowsCustomization { get; set; }
        // null لو مش بيقبل customization
        public ProductCustomizationOptionsDto? CustomizationOptions { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }
        public string ApprovalStatusText { get; set; }
        public string RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }
public decimal BasePrice { get; set; }
        public List<ProductVariantDto>? Variants { get; set; }
          public decimal? AiSuggestedPrice { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? PriceReasoning { get; set; }

    }
}