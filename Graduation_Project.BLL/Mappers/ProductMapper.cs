using Graduation_Project.BLL.DTOs.Product;
using Graduation_Project.DAL.Models.Entities;

namespace Graduation_Project.BLL.Mappers
{
    public static class ProductMapper
    {
        public static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Description = product.Description,
                ImageUrls = product.ImageUrls,
                BasePrice = product.BasePrice,
                IsActive = product.IsActive
            };
        }
    }
}