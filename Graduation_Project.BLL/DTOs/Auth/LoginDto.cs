using System.ComponentModel.DataAnnotations;

namespace Graduation_Project.BLL.DTOs.Auth
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}