using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graduation_Project.BLL.DTOs.Wishlist
{
    public class WishlistItemDto
    {
        public int WishlistItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string BrandName { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
