namespace Graduation_Project.BLL.DTOs.Product
{
    public class ImageValidationResultDto
    {
        public bool IsValid { get; set; }
        public string Status { get; set; }   // approved / rejected / error
        public string? Message { get; set; }
        public double? AiConfidence { get; set; }
        public string? Details { get; set; }
    }
}