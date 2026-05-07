using System.ComponentModel.DataAnnotations;

namespace Graduation_Project.BLL.DTOs.Order
{
    public class CreditCardDto
    {
        [Required]
        [StringLength(16, MinimumLength = 16)]
        public string CardNumber { get; set; }

        [Required]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/([0-9]{2})$",
            ErrorMessage = "Invalid format. Use MM/YY")]
        public string ExpiryDate { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string CVC { get; set; }

        [Required]
        public string CardHolderName { get; set; }
    }
}