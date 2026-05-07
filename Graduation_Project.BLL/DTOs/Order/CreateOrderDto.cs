using System.ComponentModel.DataAnnotations;
using Graduation_Project.DAL.Models.Enums;

namespace Graduation_Project.BLL.DTOs.Order
{
    public class CreateOrderDto : IValidatableObject
    {
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Shipping address is required")]
        public string ShippingAddress { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        // ✅ مش Required - بس مطلوب لو CreditCard
        public CreditCardDto? CreditCard { get; set; }

        // ✅ Validation مخصص
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PaymentMethod == PaymentMethod.CreditCard && CreditCard == null)
            {
                yield return new ValidationResult(
                    "Credit card details are required for online payment",
                    new[] { nameof(CreditCard) }
                );
            }
        }
    }
}