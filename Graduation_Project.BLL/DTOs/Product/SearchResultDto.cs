using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Graduation_Project.BLL.DTOs.Product
{
    public class SearchResultDto
    {
        public List<ProductSearchItemDto> Products { get; set; } = new();
        public List<BrandSearchItemDto> Brands { get; set; } = new();
        public List<CategorySearchItemDto> Categories { get; set; } = new();
    }

    public class ProductSearchItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal BasePrice { get; set; }
        public string? ImageUrls { get; set; }
        public string? BrandName { get; set; }
        public string? CategoryName { get; set; }
        public decimal AverageRating { get; set; }
    }

    public class BrandSearchItemDto
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public string? LogoUrl { get; set; }
    }

    public class CategorySearchItemDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}