using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.Cart;

namespace Graduation_Project.BLL.Services.Interfaces
{
    public interface ICartService
    {
        Task<ServiceResult<CartSummaryDto>> GetCartAsync(int userId);
        Task<ServiceResult<CartItemDto>> AddToCartAsync(int userId, AddToCartDto dto);
        Task<ServiceResult<CartItemDto>> UpdateQuantityAsync(int userId, int cartItemId, int quantity);
        Task<ServiceResult<bool>> RemoveFromCartAsync(int userId, int cartItemId);
        Task<ServiceResult<bool>> ClearCartAsync(int userId);
    }
}