using Graduation_Project.DAL.Models.Enums;

namespace Graduation_Project.DAL.Models.Entities
{
    public class PrintingTechnique
    {
        public int TechniqueId { get; set; }
        public PrintingTechniqueType TechniqueType { get; set; }
        public string Name { get; set; }        // "DTF - S"
        public string Dimensions { get; set; }  // "10x10cm"
        public decimal Price { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation
        public virtual ICollection<OrderItemCustomization> OrderItemCustomizations { get; set; }
    }
}