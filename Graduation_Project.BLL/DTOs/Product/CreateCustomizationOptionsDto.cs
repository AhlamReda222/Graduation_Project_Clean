namespace Graduation_Project.BLL.DTOs.Product
{
    public class CreateCustomizationOptionsDto
    {
        // الأماكن المتاحة للطباعة (Front, Back, etc.)
        public List<int> Zones { get; set; } = new();

        // بيقبل طباعة صورة
        public bool AllowsPrinting { get; set; } = false;

        // بيقبل نص
        public bool AllowsText { get; set; } = false;
    }
}