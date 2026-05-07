using System.ComponentModel.DataAnnotations;

namespace Graduation_Project.BLL.DTOs.Product
{
    public class CreateProductVariantDto
    {
        [Required]
        public string Size { get; set; }

        public string Color { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        public string SKU { get; set; }
    }
}