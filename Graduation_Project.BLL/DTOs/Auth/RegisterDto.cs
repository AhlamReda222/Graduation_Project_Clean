using System.ComponentModel.DataAnnotations;

namespace Graduation_Project.BLL.DTOs.Auth
{
    public class RegisterDto
    {
        [Required]
        public string? FullName { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [MinLength(6)]
        public string? Password { get; set; }
        
    }
}