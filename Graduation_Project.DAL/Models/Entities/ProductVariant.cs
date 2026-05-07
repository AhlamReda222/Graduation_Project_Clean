namespace Graduation_Project.DAL.Models.Entities
{
    public class ProductVariant
    {
        public int VariantId { get; set; }
        public int ProductId { get; set; }
        public string Size { get; set; }      // S, M, L, XL, 2XL
        public string Color { get; set; }     // اختياري
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string SKU { get; set; }

        // Navigation
        public virtual Product Product { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}