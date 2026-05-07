using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.Brand;

namespace Graduation_Project.BLL.Services.Interfaces
{
    public interface IBrandService
    {
        Task<ServiceResult<BrandDto>> GetBrandByIdAsync(int brandId);
        Task<ServiceResult<List<BrandDto>>> GetAllBrandsAsync();
        Task<ServiceResult<List<BrandDto>>> GetBrandsByOwnerAsync(int userId);
        Task<ServiceResult<BrandDto>> UpdateBrandAsync(int brandId, int userId, UpdateBrandDto dto);
        Task<ServiceResult<bool>> DeleteBrandAsync(int brandId, int userId);
        Task<ServiceResult<bool>> ToggleBrandStatusAsync(int brandId, int adminId);
    }
}