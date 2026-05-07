namespace Graduation_Project.BLL.DTOs.Order
{
    public class OrderItemCustomizationDto
    {
        public string Zone { get; set; }
        public string TechniqueName { get; set; }
        public string? DesignImageUrl { get; set; }
        public string? DesignText { get; set; }
        public decimal TechniquePrice { get; set; }
    }
}