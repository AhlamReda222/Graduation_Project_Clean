
// ============================================================
// 📁 DTOs/Cart/AddToCartDto.cs
// ============================================================

using System.ComponentModel.DataAnnotations;

namespace Graduation_Project.BLL.DTOs.Cart
{
    public class AddToCartDto
    {
        [Required]
        public int ProductId { get; set; }

        // ✅ nullable - مش كل منتج عنده variant
        public int? VariantId { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
        public int Quantity { get; set; }

        // ✅ nullable - مش كل منتج بيسمح بـ customization
        public CartCustomizationDto? Customization { get; set; }
    }
}