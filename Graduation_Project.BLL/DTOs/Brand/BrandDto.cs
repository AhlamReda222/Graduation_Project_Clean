using Graduation_Project.BLL.DTOs.Product;
namespace Graduation_Project.BLL.DTOs.Brand
{
    public class BrandDto
    {
        public int BrandId { get; set; }
        public int UserId { get; set; }
        public string OwnerName { get; set; }
        public string BrandName { get; set; }
        public string Description { get; set; }
        public string LogoUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public int ProductCount { get; set; }
        public List<ProductDto> Products { get; set; } = new();
    }
}