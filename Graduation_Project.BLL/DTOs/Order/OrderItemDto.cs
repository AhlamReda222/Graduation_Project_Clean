namespace Graduation_Project.BLL.DTOs.Order
{
    public class OrderItemDto
    {
        public int OrderItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string BrandName { get; set; }
        public int? VariantId { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal CustomizationPrice { get; set; }
        public decimal Subtotal { get; set; }
        public OrderItemCustomizationDto Customization { get; set; }
    }
}