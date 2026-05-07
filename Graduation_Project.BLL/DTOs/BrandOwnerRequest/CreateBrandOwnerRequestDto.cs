using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Abstractions;
namespace Graduation_Project.BLL.DTOs.BrandOwnerRequest
{
    public class CreateBrandOwnerRequestDto
    {
        // بيانات الطلب
        [Required(ErrorMessage = "Business name is required")]
        [MaxLength(100)]
        public string BusinessName { get; set; }

        [Required(ErrorMessage = "Business license is required")]
public IFormFile BusinessLicense { get; set; }
public IFormFile BrandLogo { get; set; }

        // ✅ بيانات البراند في نفس الطلب
        [Required(ErrorMessage = "Brand name is required")]
        [MaxLength(100)]
        public string BrandName { get; set; }

        [MaxLength(500)]
        public string BrandDescription { get; set; }

    }
}