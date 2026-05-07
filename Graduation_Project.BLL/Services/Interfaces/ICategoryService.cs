using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.Category;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Graduation_Project.BLL.Services.Interfaces
{
    public interface ICategoryService
    {
        // CRUD Operations
        Task<ServiceResult<CategoryDto>> CreateCategoryAsync(CreateCategoryDto dto, int adminId);
        Task<ServiceResult<CategoryDto>> GetCategoryByIdAsync(int categoryId);
        Task<ServiceResult<CategoryDto>> UpdateCategoryAsync(int categoryId, UpdateCategoryDto dto, int adminId);
        Task<ServiceResult<bool>> DeleteCategoryAsync(int categoryId, int adminId);

        // Listing
        Task<ServiceResult<List<CategoryDto>>> GetAllCategoriesAsync();

        // Statistics
        Task<ServiceResult<CategoryStatsDto>> GetCategoryStatsAsync(int categoryId);
    }
}