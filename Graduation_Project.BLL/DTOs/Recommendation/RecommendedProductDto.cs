 // المنتج المرشح من الـ AI
      namespace Graduation_Project.BLL.DTOs.Recommendation
{
    public class RecommendedProductDto
    {
        public int    ProductId   { get; set; }
        public string ProductName { get; set; } = null!;
        public string? BrandName  { get; set; }
        public decimal Price      { get; set; }
        public string? ImageUrl   { get; set; }
        public double? Score      { get; set; } // درجة التشابه
    }
}