using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.Product;

namespace Graduation_Project.BLL.Services.Interfaces
{
    public interface IProductService
    {
        Task<ServiceResult<ProductDto>> CreateProductAsync(int userId, int brandId, CreateProductDto dto);
        Task<ServiceResult<ProductDto>> GetProductByIdAsync(int productId);
        Task<ServiceResult<List<ProductDto>>> GetAllApprovedProductsAsync();
        Task<ServiceResult<List<ProductDto>>> GetProductsByBrandAsync(int brandId);
        Task<ServiceResult<List<ProductDto>>> GetOwnerProductsAsync(int userId);
        Task<ServiceResult<List<ProductDto>>> GetPendingProductsAsync();
        Task<ServiceResult<ProductDto>> UpdateProductAsync(int productId, int userId, UpdateProductDto dto);
        Task<ServiceResult<bool>> DeleteProductAsync(int productId, int userId);
        Task<ServiceResult<ProductDto>> ApproveProductAsync(int productId, string aiResponse);
        Task<ServiceResult<List<PrintingTechniqueDto>>> GetPrintingTechniquesAsync();
    }
}