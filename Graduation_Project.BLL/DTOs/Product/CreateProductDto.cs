using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
 
namespace Graduation_Project.BLL.DTOs.Product
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "Product name is required")]
        [MaxLength(200)]
        public string ProductName { get; set; }
 
        [Required(ErrorMessage = "Description is required")]
        [MaxLength(2000)]
        public string Description { get; set; }
 
        [Required]
        public int CategoryId { get; set; }
 
        // للمنتجات بدون variants
        public decimal? BasePrice { get; set; }
        public int? StockQuantity { get; set; }
 
        // للمنتجات بـ variants (JSON string)
        public string? VariantsJson { get; set; }
 
        // الصور
        [Required(ErrorMessage = "At least one image is required")]
        public List<IFormFile>? Images { get; set; }
 
        // Customization (optional)
        public CreateCustomizationOptionsDto? Customization { get; set; }
 
        // ✅ الأونر يقرر هل يستخدم السعر المقترح من AI ولا لا
        public bool UseAiSuggestion { get; set; } = false;
 
        // ✅ السعر المقترح من الـ Real-time predict-price endpoint
        public decimal? AiSuggestedPrice { get; set; }
    }
}