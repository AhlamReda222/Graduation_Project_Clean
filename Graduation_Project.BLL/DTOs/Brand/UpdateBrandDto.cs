using System.ComponentModel.DataAnnotations;

namespace Graduation_Project.BLL.DTOs.Brand
{
    public class UpdateBrandDto
    {
        [Required(ErrorMessage = "Brand name is required")]
        [MaxLength(100)]
        public string BrandName { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public string LogoUrl { get; set; }

        public bool IsActive { get; set; }
    }
}