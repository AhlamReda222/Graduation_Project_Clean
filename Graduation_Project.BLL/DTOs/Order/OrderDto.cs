using Graduation_Project.DAL.Models.Enums;

namespace Graduation_Project.BLL.DTOs.Order
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal FinalTotal { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string OrderStatusText { get; set; }
        public string ShippingAddress { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string PaymentMethodText { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string PaymentStatusText { get; set; }
        public string TransactionId { get; set; }
        public string TrackingNumber { get; set; }
        public string PaymentMessage { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }
}