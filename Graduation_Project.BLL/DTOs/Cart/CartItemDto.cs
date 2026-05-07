namespace Graduation_Project.BLL.DTOs.Cart
{
  
    public class CartItemDto
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductImage { get; set; }
        public string? BrandName { get; set; }

        // Variant Info - null لو مفيش variant
        public int? VariantId { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }

        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }

        public bool HasCustomization { get; set; }
        public decimal CustomizationPrice { get; set; }

        public decimal Subtotal { get; set; }

        // Customization Info - null لو مفيش customization
        public CartItemCustomizationDto? Customization { get; set; }
    }
}