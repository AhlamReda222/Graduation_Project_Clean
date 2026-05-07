using Graduation_Project.DAL.Models.Enums;
using Microsoft.AspNetCore.Http;

namespace Graduation_Project.BLL.DTOs.BrandOwnerRequest
{
    public class BrandOwnerRequestDto
    {
        public int RequestId { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? BusinessName { get; set; }
        public string? BusinessLicense { get; set; }

        // ✅ بيانات البراند
        public string? BrandName { get; set; }
        public string? BrandDescription { get; set; }
        public string? BrandLogo { get; set; }
        public int? CreatedBrandId { get; set; } // الـ ID بتاع البراند اللي اتعمل

        public RequestStatus RequestStatus { get; set; }
        public string? RequestStatusText { get; set; }
        public DateTime RequestDate { get; set; }
        public int? ReviewedBy { get; set; }
        public string? ReviewerName { get; set; }
        public DateTime? ReviewDate { get; set; }
    }
}