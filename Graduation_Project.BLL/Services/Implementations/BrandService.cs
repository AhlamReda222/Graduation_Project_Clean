using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.Brand;
using Graduation_Project.BLL.Services.Interfaces;
using Graduation_Project.DAL.Models.Entities;
using Graduation_Project.DAL.Models.Enums;
using Graduation_Project.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Graduation_Project.BLL.Mappers;
namespace Graduation_Project.BLL.Services.Implementations
{
    public class BrandService : IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BrandService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<ServiceResult<BrandDto>> GetBrandByIdAsync(int brandId)
        {
            try
            {
                var brand = await _unitOfWork.Brands
                    .GetQueryable()
                    .Include(b => b.User)
                    .Include(b => b.Products)
                                .ThenInclude(p => p.Variants) // 👈 يتحط هنا
                    .FirstOrDefaultAsync(b => b.BrandId == brandId);
                    

                if (brand == null)
                    return ServiceResult<BrandDto>.Failure("Brand not found");

                return ServiceResult<BrandDto>.Success(MapToDto(brand));
            }
            catch (Exception ex)
            {
                return ServiceResult<BrandDto>.Failure($"Error fetching brand: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<BrandDto>>> GetAllBrandsAsync()
        {
            try
            {
                var brands = await _unitOfWork.Brands
                    .GetQueryable()
                    .Include(b => b.User)
                    .Include(b => b.Products)
                    .OrderByDescending(b => b.CreatedAt)
                    .ToListAsync();

                return ServiceResult<List<BrandDto>>.Success(brands.Select(MapToDto).ToList());
            }
            catch (Exception ex)
            {
                return ServiceResult<List<BrandDto>>.Failure($"Error fetching brands: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<BrandDto>>> GetBrandsByOwnerAsync(int userId)
        {
            try
            {
                var brands = await _unitOfWork.Brands
                    .GetQueryable()
                    .Include(b => b.User)
                    .Include(b => b.Products)
                    .Where(b => b.UserId == userId)
                    .OrderByDescending(b => b.CreatedAt)
                    .ToListAsync();

                return ServiceResult<List<BrandDto>>.Success(brands.Select(MapToDto).ToList());
            }
            catch (Exception ex)
            {
                return ServiceResult<List<BrandDto>>.Failure($"Error fetching brands: {ex.Message}");
            }
        }

        public async Task<ServiceResult<BrandDto>> UpdateBrandAsync(int brandId, int userId, UpdateBrandDto dto)
        {
            try
            {
                var brand = await _unitOfWork.Brands
                    .GetQueryable()
                    .Include(b => b.User)
                    .Include(b => b.Products)
                    .FirstOrDefaultAsync(b => b.BrandId == brandId);

                if (brand == null)
                    return ServiceResult<BrandDto>.Failure("Brand not found");

                var user = await _unitOfWork.ApplicationUsers.GetByIdAsync(userId);
                if (user == null)
                    return ServiceResult<BrandDto>.Failure("User not found");

                if (user.UserType == UserType.BrandOwner && brand.UserId != userId)
                    return ServiceResult<BrandDto>.Failure("You can only update your own brands");

                var nameExists = await _unitOfWork.Brands
                    .AnyAsync(b => b.BrandName.ToLower() == dto.BrandName.ToLower()
                                && b.UserId == brand.UserId
                                && b.BrandId != brandId);

                if (nameExists)
                    return ServiceResult<BrandDto>.Failure("You already have a brand with this name");

                brand.BrandName = dto.BrandName;
                brand.Description = dto.Description;
                brand.LogoUrl = dto.LogoUrl;
                brand.IsActive = dto.IsActive;

                _unitOfWork.Brands.Update(brand);
                await _unitOfWork.SaveAsync();

                return ServiceResult<BrandDto>.Success(MapToDto(brand));
            }
            catch (Exception ex)
            {
                return ServiceResult<BrandDto>.Failure($"Error updating brand: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteBrandAsync(int brandId, int userId)
        {
            try
            {
                var brand = await _unitOfWork.Brands
                    .GetQueryable()
                    .Include(b => b.Products)
                    .FirstOrDefaultAsync(b => b.BrandId == brandId);

                if (brand == null)
                    return ServiceResult<bool>.Failure("Brand not found");

                var user = await _unitOfWork.ApplicationUsers.GetByIdAsync(userId);
                if (user == null)
                    return ServiceResult<bool>.Failure("User not found");

                if (user.UserType == UserType.BrandOwner && brand.UserId != userId)
                    return ServiceResult<bool>.Failure("You can only delete your own brands");

                if (brand.Products != null && brand.Products.Any())
                    return ServiceResult<bool>.Failure("Cannot delete brand with products. Delete products first.");

                _unitOfWork.Brands.Delete(brand);
                await _unitOfWork.SaveAsync();

                return ServiceResult<bool>.Success(true, "Brand deleted successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure($"Error deleting brand: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ToggleBrandStatusAsync(int brandId, int adminId)
        {
            try
            {
                var brand = await _unitOfWork.Brands.GetByIdAsync(brandId);
                if (brand == null)
                    return ServiceResult<bool>.Failure("Brand not found");

                brand.IsActive = !brand.IsActive;
                _unitOfWork.Brands.Update(brand);
                await _unitOfWork.SaveAsync();

                var status = brand.IsActive ? "activated" : "deactivated";
                return ServiceResult<bool>.Success(true, $"Brand {status} successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure($"Error toggling brand status: {ex.Message}");
            }
        }
    
    private BrandDto MapToDto(Brand brand)
{
    return new BrandDto
    {
        BrandId = brand.BrandId,
        BrandName = brand.BrandName,
        Description = brand.Description,
        IsActive = brand.IsActive,
        OwnerName = brand.User?.FullName,

        Products = brand.Products?
            .Where(p => p.IsActive && p.ApprovalStatus == ApprovalStatus.Approved) // 👈 مهم
            .Select(p => ProductMapper.MapToDto(p)) // 👈 من الـ Mapper
            .ToList() ?? new()
    };
}
}}