using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graduation_Project.BLL.DTOs.Product;
namespace Graduation_Project.BLL.DTOs.Category
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public int ProductCount { get; set; }
         public List<ProductDto> Products { get; set; } = new(); // 👈 الجديد

    }
}
