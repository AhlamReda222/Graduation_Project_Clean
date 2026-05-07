using Graduation_Project.DAL.Models.Entities;
using Graduation_Project.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graduation_Project.DAL.Models.Entities
{
    public class Product
    {
        public int ProductId { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public bool AllowsCustomization { get; set; } = false; // ✅ الجديد
        public bool AllowsPrinting { get; set; } = false;   // ✅ الجديد
        public bool AllowsText { get; set; } = false;        // ✅ الجديد
        public decimal BasePrice { get; set; }
        public string? RejectionReason { get; set; }           // ✅ الجديد - سبب الرفض من الـ AI
        public string ImageUrls { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public int StockQuantity { get; set; } // ✅ ضيفي ده
public decimal? AiSuggestedPrice { get; set; }
public decimal? MinPrice { get; set; }
public decimal? MaxPrice { get; set; }
public string? PriceReasoning { get; set; }        // Navigation Properties
        public virtual Brand Brand { get; set; }
        public virtual Category Category { get; set; }
        public virtual ApplicationUser ApprovedByUser { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Discount> Discounts { get; set; }
         public virtual ICollection<ProductVariant> Variants { get; set; }              // ✅ الجديد
        public virtual ICollection<ProductCustomizationZone> CustomizationZones { get; set; } // ✅ الجديد
    }
}
