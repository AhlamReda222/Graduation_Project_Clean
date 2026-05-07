using Graduation_Project.DAL.Models.Enums;

namespace Graduation_Project.DAL.Models.Entities
{
    public class OrderItemCustomization
    {
        public int CustomizationId { get; set; }
        public int OrderItemId { get; set; }
        public CustomizationZone Zone { get; set; }      // Front, Back, etc.
        public int TechniqueId { get; set; }
        public string? DesignImageUrl { get; set; }       // الصورة اللي رفعها الكاستمر
        public string? DesignText { get; set; }           // لو حط نص
        public decimal CustomizationPrice { get; set; }  // السعر وقت الطلب

        // Navigation
        public virtual OrderItem OrderItem { get; set; }
        public virtual PrintingTechnique Technique { get; set; }
    }
}