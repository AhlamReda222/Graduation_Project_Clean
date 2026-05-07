namespace Graduation_Project.BLL.DTOs.Product
{
    public class ProductVariantDto
    {
        public int VariantId { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string SKU { get; set; }
    }
}