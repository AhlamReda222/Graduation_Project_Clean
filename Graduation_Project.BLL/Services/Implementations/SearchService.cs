using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.Product;
using Graduation_Project.BLL.Services.Interfaces;
using Graduation_Project.DAL.Models.Enums;
using Graduation_Project.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Graduation_Project.BLL.Services.Implementations
{
    public class SearchService : ISearchService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SearchService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<SearchResultDto>> SearchAsync(string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                    return ServiceResult<SearchResultDto>.Failure("Search query is required.");

                query = query.ToLower().Trim();

                var products = await _unitOfWork.Products
                    .GetQueryable()
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Where(p => p.IsActive
                             && p.ApprovalStatus == ApprovalStatus.Approved
                             && p.ProductName.ToLower().Contains(query))
                    .Select(p => new ProductSearchItemDto
                    {
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        BasePrice = p.BasePrice,
                        ImageUrls = p.ImageUrls,
                        BrandName = p.Brand.BrandName,
                        CategoryName = p.Category.CategoryName,
                        AverageRating = p.AverageRating
                    })
                    .Take(10)
                    .ToListAsync();

                var brands = await _unitOfWork.Brands
                    .GetQueryable()
                    .Where(b => b.IsActive
                             && b.BrandName.ToLower().Contains(query))
                    .Select(b => new BrandSearchItemDto
                    {
                        BrandId = b.BrandId,
                        BrandName = b.BrandName,
                        LogoUrl = b.LogoUrl
                    })
                    .Take(5)
                    .ToListAsync();

                var categories = await _unitOfWork.Categories
                    .GetQueryable()
                    .Where(c => c.CategoryName.ToLower().Contains(query))
                    .Select(c => new CategorySearchItemDto
                    {
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName
                    })
                    .Take(5)
                    .ToListAsync();

                var result = new SearchResultDto
                {
                    Products = products,
                    Brands = brands,
                    Categories = categories
                };

                return ServiceResult<SearchResultDto>.Success(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<SearchResultDto>.Failure($"Error: {ex.Message}");
            }
        }
    }
}