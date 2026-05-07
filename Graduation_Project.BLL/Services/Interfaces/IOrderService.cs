using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.Order;
using Graduation_Project.DAL.Models.Enums;

namespace Graduation_Project.BLL.Services.Interfaces
{
    public interface IOrderService
    {
        Task<ServiceResult<OrderDto>> CreateOrderFromCartAsync(int userId, CreateOrderDto dto);
        Task<ServiceResult<OrderDto>> GetOrderByIdAsync(int orderId, int userId);
        Task<ServiceResult<List<OrderDto>>> GetMyOrdersAsync(int userId);
        Task<ServiceResult<List<OrderDto>>> GetAllOrdersAsync();
        Task<ServiceResult<OrderDto>> UpdateOrderStatusAsync(int orderId, OrderStatus status);
        Task<ServiceResult<OrderDto>> CancelOrderAsync(int orderId, int userId);
    }
}