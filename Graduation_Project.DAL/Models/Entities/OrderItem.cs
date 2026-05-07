using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graduation_Project.DAL.Models.Entities
{
    public class OrderItem
{
    public int OrderItemId { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int? VariantId { get; set; }  // ✅ الجديد
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal CustomizationPrice { get; set; }  // ✅ الجديد
    public decimal Subtotal { get; set; }

    public virtual Order Order { get; set; }
    public virtual Product Product { get; set; }
    public virtual ProductVariant ProductVariant { get; set; }  // ✅ الجديد
    public virtual ICollection<OrderItemCustomization> Customizations { get; set; }  // ✅ الجديد
}
}