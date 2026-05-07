namespace Graduation_Project.BLL.DTOs.Product
{
    public class ProductCustomizationOptionsDto
    {
        public bool AllowsPrinting { get; set; }
        public bool AllowsText { get; set; }
        public List<string>? AvailableZones { get; set; }
    }
}