namespace Graduation_Project.BLL.DTOs.Product
{
    public class AiModerationResultDto
    {
        public bool IsApproved { get; set; }
        public string Status { get; set; }
        public string? Message { get; set; }
        public string? Reason { get; set; }
    }

}