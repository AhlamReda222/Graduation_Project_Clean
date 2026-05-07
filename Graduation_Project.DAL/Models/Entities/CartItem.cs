using Graduation_Project.DAL.Models.Enums;
namespace Graduation_Project.DAL.Models.Entities
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int? VariantId { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedAt { get; set; }

        // Customization - اختياري
public CustomizationZone? CustomizationZone { get; set; }        public int? TechniqueId { get; set; }              // ✅ الجديد
        public string? DesignImageUrl { get; set; }         // ✅ الجديد
        public string? DesignText { get; set; }             // ✅ الجديد
        public string Signature { get; set; }
        // Navigation Properties
        public virtual ApplicationUser User { get; set; }
        public virtual Product Product { get; set; }
        public virtual ProductVariant ProductVariant { get; set; }
        public virtual PrintingTechnique? Technique { get; set; } // ✅ الجديد
    }
}