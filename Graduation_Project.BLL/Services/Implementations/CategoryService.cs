using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.Category;
using Graduation_Project.BLL.Services.Interfaces;
using Graduation_Project.DAL.Repositories.Interfaces;
using Graduation_Project.DAL.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Graduation_Project.BLL.Mappers;
using Graduation_Project.DAL.Models.Enums;

namespace Graduation_Project.BLL.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<CategoryDto>> CreateCategoryAsync(CreateCategoryDto dto, int adminId)
        {
            try
            {
                var nameExists = await _unitOfWork.Categories
                    .AnyAsync(c => c.CategoryName.ToLower() == dto.CategoryName.ToLower());

                if (nameExists)
                    return ServiceResult<CategoryDto>.Failure("Category name already exists");

                var category = new Category
                {
                    CategoryName = dto.CategoryName,
                    Description = dto.Description
                };

                await _unitOfWork.Categories.AddAsync(category);
                await _unitOfWork.SaveAsync();

                var result = new CategoryDto
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    Description = category.Description,
                    ProductCount = 0
                };

                return ServiceResult<CategoryDto>.Success(result, "Category created successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<CategoryDto>.Failure($"Error creating category: {ex.Message}");
            }
        }
public async Task<ServiceResult<CategoryDto>> GetCategoryByIdAsync(int categoryId)
{
    try
    {
        var category = await _unitOfWork.Categories
            .GetQueryable()
            .Include(c => c.Products)
                .ThenInclude(p => p.Brand)
            .Include(c => c.Products)
                .ThenInclude(p => p.Variants)
            .FirstOrDefaultAsync(c => c.CategoryId == categoryId);

        if (category == null)
            return ServiceResult<CategoryDto>.Failure("Category not found");

        var result = new CategoryDto
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            Description = category.Description,

            ProductCount = category.Products?.Count ?? 0,

          Products = category.Products?
    .Where(p => p.IsActive && p.ApprovalStatus == ApprovalStatus.Approved)
    .Select(p => ProductMapper.MapToDto(p))
    .ToList() ?? new()
        };

        return ServiceResult<CategoryDto>.Success(result);
    }
    catch (Exception ex)
    {
        return ServiceResult<CategoryDto>.Failure($"Error fetching category: {ex.Message}");
    }
}

        public async Task<ServiceResult<CategoryDto>> UpdateCategoryAsync(int categoryId, UpdateCategoryDto dto, int adminId)
        {
            try
            {
                var category = await _unitOfWork.Categories
                    .GetQueryable()
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.CategoryId == categoryId);

                if (category == null)
                    return ServiceResult<CategoryDto>.Failure("Category not found");

                var nameExists = await _unitOfWork.Categories
                    .AnyAsync(c => c.CategoryName.ToLower() == dto.CategoryName.ToLower()
                                && c.CategoryId != categoryId);

                if (nameExists)
                    return ServiceResult<CategoryDto>.Failure("Category name already exists");

                category.CategoryName = dto.CategoryName;
                category.Description = dto.Description;

                _unitOfWork.Categories.Update(category);
                await _unitOfWork.SaveAsync();

                var result = new CategoryDto
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    Description = category.Description,
                    ProductCount = category.Products?.Count ?? 0
                };

                return ServiceResult<CategoryDto>.Success(result, "Category updated successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<CategoryDto>.Failure($"Error updating category: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteCategoryAsync(int categoryId, int adminId)
        {
            try
            {
                var category = await _unitOfWork.Categories
                    .GetQueryable()
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.CategoryId == categoryId);

                if (category == null)
                    return ServiceResult<bool>.Failure("Category not found");

                if (category.Products != null && category.Products.Any())
                    return ServiceResult<bool>.Failure("Cannot delete category with products. Please move or delete products first.");

                _unitOfWork.Categories.Delete(category);
                await _unitOfWork.SaveAsync();

                return ServiceResult<bool>.Success(true, "Category deleted successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure($"Error deleting category: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<CategoryDto>>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _unitOfWork.Categories
                    .GetQueryable()
                    .Include(c => c.Products)
                    .Select(c => new CategoryDto
                    {
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName,
                        Description = c.Description,
                        ProductCount = c.Products.Count
                    })
                    .ToListAsync();

                return ServiceResult<List<CategoryDto>>.Success(categories);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<CategoryDto>>.Failure($"Error fetching categories: {ex.Message}");
            }
        }

        public async Task<ServiceResult<CategoryStatsDto>> GetCategoryStatsAsync(int categoryId)
        {
            try
            {
                var category = await _unitOfWork.Categories
                    .GetQueryable()
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.CategoryId == categoryId);

                if (category == null)
                    return ServiceResult<CategoryStatsDto>.Failure("Category not found");

             

                var avgRating = category.Products.Any()
                    ? category.Products.Average(p => (double)p.AverageRating)
                    : 0.0;

                var stats = new CategoryStatsDto
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    TotalProducts = category.Products.Count,
                    TotalOrders = 0,
                    TotalRevenue = 0m,
                    AverageRating = avgRating
                };

                return ServiceResult<CategoryStatsDto>.Success(stats);
            }
            catch (Exception ex)
            {
                return ServiceResult<CategoryStatsDto>.Failure($"Error fetching category stats: {ex.Message}");
            }
        }
    }
}