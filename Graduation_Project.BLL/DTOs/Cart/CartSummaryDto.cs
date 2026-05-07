namespace Graduation_Project.BLL.DTOs.Cart
{
   
    public class CartSummaryDto
    {
        public List<CartItemDto> Items { get; set; } = new();
        public int TotalItems { get; set; }
        public decimal SubTotal { get; set; }
        public decimal CustomizationTotal { get; set; }
        public decimal Total { get; set; }
    }

}