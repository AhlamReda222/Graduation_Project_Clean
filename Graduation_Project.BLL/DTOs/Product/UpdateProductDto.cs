using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Graduation_Project.BLL.DTOs.Product
{
    public class UpdateProductDto
    {
        [Required]
        [MaxLength(200)]
        public string ProductName { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        public List<IFormFile>? Images { get; set; }
        public int CategoryId { get; set; }

        public bool AllowsCustomization { get; set; }

        public bool IsActive { get; set; }

        public List<CreateProductVariantDto>? Variants { get; set; }
            public CreateCustomizationOptionsDto? Customization { get; set; }

public decimal BasePrice { get; set; }
    public int? StockQuantity { get; set; } // ✅ لازم تضيفيها

        public List<int> CustomizationZones { get; set; } = new();
    }
}