using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.Product;

namespace Graduation_Project.BLL.Services.Interfaces
{
    public interface IProductDescriptionService
    {
        Task<ServiceResult<string>> GenerateDescriptionAsync(GenerateDescriptionDto dto);
    }
}